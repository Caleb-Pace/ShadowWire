using Microsoft.Data.Sqlite;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Users;

namespace ShadowWire.Desktop.Client.Storage;

// TODO: (Post-MVP) handle self messaging cleanly.
// TODO: (Post-MVP) get timestamp for sent message from server.
internal sealed class SqliteConversationRepository : IConversationRepository
{
    private readonly string _connectionString;
    private readonly Fingerprint _myFingerprint;
    private readonly byte[] _myFingerprintBytes;



    public SqliteConversationRepository(string dbPath, Fingerprint self)
    {
        this._connectionString   = $"Data Source={dbPath}";
        this._myFingerprint      = self;
        this._myFingerprintBytes = self.ToArray();
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await using var conn = await OpenAsync(ct);

        // Use Write-Ahead Logging mode for better concurrency
        await using var pragma = conn.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode=WAL;";
        await pragma.ExecuteNonQueryAsync(ct);

        // Ensure tables are created
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Items (
                LocalId     INTEGER PRIMARY KEY AUTOINCREMENT,
                Kind        INTEGER NOT NULL,   -- 1 byte
                Source      BLOB    NOT NULL,   -- 32 bytes
                Destination BLOB    NOT NULL,   -- 32 bytes
                Timestamp   INTEGER NOT NULL,   -- UTC ticks, server-assigned
                IsRead      INTEGER NOT NULL DEFAULT 0,
                Raw         BLOB    NOT NULL    -- exactly as received
            );
            CREATE INDEX IF NOT EXISTS IX_Items_Conversation
                ON Items (Source, Destination, Timestamp);
            CREATE INDEX IF NOT EXISTS IX_Items_Unread
                ON Items (Destination, Source) WHERE IsRead = 0;
            """;
        await cmd.ExecuteNonQueryAsync(ct);
    }


    // TODO: Add comment
    private async Task<SqliteConnection> OpenAsync(CancellationToken ct)
    {
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }

    // TODO: Add comment
    private static async Task<IReadOnlyList<StoredEntry<T>>> ReadAllAsync<T>(
        SqliteCommand cmd,
        CancellationToken ct
    ) where T : struct, IConversationStorable<T>
    {
        var results = new List<StoredEntry<T>>();

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var localId = reader.GetInt64(0);
            var isRead  = (reader.GetInt64(1) != 0);
            var raw     = reader.GetFieldValue<byte[]>(2); // Protocol Message in Bytes

            results.Add(new StoredEntry<T>(localId, T.Decode(raw), isRead));
        }
        
        return results;
    }


    public async Task StoreAsync<T>(T item, ReadOnlyMemory<byte> raw, CancellationToken ct = default)
        where T : struct, IConversationStorable<T>
    {
        var isRead = item.Source.Equals(_myFingerprint);

        await using var conn = await OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Items (Kind, Source, Destination, Timestamp, IsRead, Raw)
            VALUES ($kind, $src, $dst, $ts, $read, $raw);
            """;
        cmd.Parameters.AddWithValue("$kind", (byte)T.Kind);
        cmd.Parameters.AddWithValue("$src",  item.Source.ToArray());
        cmd.Parameters.AddWithValue("$dst",  item.Destination.ToArray());
        cmd.Parameters.AddWithValue("$ts",   item.Timestamp.UtcTicks);
        cmd.Parameters.AddWithValue("$read", (isRead ? 1 : 0));
        cmd.Parameters.AddWithValue("$raw",  raw.ToArray());
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<StoredEntry<T>>> GetMessagesAsync<T>(
        Fingerprint peer,
        int limit,
        ConversationCursor? before = null,
        CancellationToken ct = default
    ) where T : struct, IConversationStorable<T>
    {
        await using var conn = await OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT LocalId, IsRead, Raw FROM Items
            WHERE Kind = $kind
              AND ((Source = $peer AND Destination = $me)
              OR   (Source = $me   AND Destination = $peer))
              AND (Timestamp, LocalId) < ($beforeTs, $beforeId)
            ORDER BY Timestamp DESC, LocalId DESC
            LIMIT $limit;
            """;
        var cursor = before ?? new ConversationCursor(long.MaxValue, long.MaxValue);
        cmd.Parameters.AddWithValue("$kind",     (byte)T.Kind);
        cmd.Parameters.AddWithValue("$peer",     peer.ToArray());
        cmd.Parameters.AddWithValue("$me",       _myFingerprintBytes);
        cmd.Parameters.AddWithValue("$beforeTs", cursor.Ticks);
        cmd.Parameters.AddWithValue("$beforeId", cursor.LocalId);
        cmd.Parameters.AddWithValue("$limit",    limit);
        return await ReadAllAsync<T>(cmd, ct);
    }

    public async Task<IReadOnlyList<StoredEntry<T>>> GetUnreadAsync<T>(
        Fingerprint? from = null,
        CancellationToken ct = default
    ) where T : struct, IConversationStorable<T>
    {
        await using var conn = await OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT LocalId, IsRead, Raw FROM Items
            WHERE Kind = $kind
              AND Destination = $me
              AND IsRead = 0
              AND ($from IS NULL OR Source = $from)
            ORDER BY Timestamp, LocalId;
            """;
        cmd.Parameters.AddWithValue("$kind", (byte)T.Kind);
        cmd.Parameters.AddWithValue("$me",   _myFingerprintBytes);
        cmd.Parameters.AddWithValue("$from", ( (from is { } f) ? f.ToArray() : DBNull.Value ));
        return await ReadAllAsync<T>(cmd, ct);
    }

    public async Task MarkReadAsync(Fingerprint peer, DateTimeOffset upTo, CancellationToken ct = default)
    {
        await using var conn = await OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE Items SET IsRead = 1
            WHERE Source = $peer
              AND Destination = $me
              AND IsRead = 0
              AND Timestamp <= $upTo;
            """;
        cmd.Parameters.AddWithValue("$peer", peer.ToArray());
        cmd.Parameters.AddWithValue("$me",   _myFingerprintBytes);
        cmd.Parameters.AddWithValue("$upTo", upTo.UtcTicks);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
