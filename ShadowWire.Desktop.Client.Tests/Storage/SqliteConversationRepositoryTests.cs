using Microsoft.Data.Sqlite;
using ShadowWire.Desktop.Client.Storage;
using ShadowWire.Shared.Users;
using ShadowWire.Shared.Protocol.Messages;

namespace ShadowWire.Desktop.Client.Tests.Storage;

public sealed class SqliteConversationRepositoryTests : IAsyncLifetime
{
    private static readonly Fingerprint Me  = Fp(1);
    private static readonly Fingerprint Sam = Fp(2);
    private static readonly Fingerprint Amy = Fp(3);

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
    private SqliteConversationRepository _repo = null!;


    public async ValueTask InitializeAsync()
    {
        _repo = new SqliteConversationRepository(_dbPath, Me);
        await _repo.InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        SqliteConnection.ClearAllPools();   // release pooled file handles first
        foreach (var path in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
            if (File.Exists(path)) File.Delete(path);
        return;
    }

    private static Fingerprint Fp(byte id)
        => Fingerprint.FromSpan(new byte[Fingerprint.SIZE].Select(_ => id).ToArray());

    private static Message Msg(Fingerprint from, Fingerprint to, long ticks)
        => new Message(from, to, new DateTimeOffset(ticks, TimeSpan.Zero), []);

    private Task Store(Message m)
        => _repo.StoreAsync(m, m.Encode());


    [Fact]
    public async Task Stored_incoming_message_is_returned_and_unread()
    {
        var m = Msg(Sam, Me, ticks: 1000);
        await Store(m);

        var convo  = await _repo.GetMessagesAsync<Message>(Sam, limit: 10, ct: TestContext.Current.CancellationToken);
        var unread = await _repo.GetUnreadAsync<Message>(ct: TestContext.Current.CancellationToken);

        Assert.Single(convo);
        Assert.False(convo[0].IsRead);
        Assert.Single(unread);
        Assert.Equal(m.Encode(), convo[0].Item.Encode());   // round-trips through Raw
    }

    [Fact]
    public async Task Sent_message_is_stored_as_read_and_shows_in_conversation()
    {
        await Store(Msg(Me, Sam, 1000));

        Assert.Empty(await _repo.GetUnreadAsync<Message>(ct: TestContext.Current.CancellationToken));
        var convo = await _repo.GetMessagesAsync<Message>(Sam, 10, ct: TestContext.Current.CancellationToken);
        Assert.Single(convo);
        Assert.True(convo[0].IsRead);
    }

    [Fact]
    public async Task MarkRead_clears_unread_up_to_timestamp_only()
    {
        await Store(Msg(Sam, Me, 1000));
        await Store(Msg(Sam, Me, 2000));
        await Store(Msg(Sam, Me, 3000));

        await _repo.MarkReadAsync(Sam, new DateTimeOffset(2000, TimeSpan.Zero), ct: TestContext.Current.CancellationToken);

        var unread = await _repo.GetUnreadAsync<Message>(ct: TestContext.Current.CancellationToken);
        Assert.Single(unread);
        Assert.Equal(3000, unread[0].Item.Timestamp.UtcTicks);
    }

    [Fact]
    public async Task Conversations_are_isolated_per_peer()
    {
        await Store(Msg(Sam, Me, 1000));
        await Store(Msg(Amy, Me, 1000));

        Assert.Single(await _repo.GetMessagesAsync<Message>(Sam, 10, ct: TestContext.Current.CancellationToken));
        Assert.Single(await _repo.GetUnreadAsync<Message>(from: Amy, ct: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Conversation_is_newest_first()
    {
        await Store(Msg(Sam, Me, 1000));
        await Store(Msg(Me, Sam, 2000));
        await Store(Msg(Sam, Me, 3000));

        var ticks = (await _repo.GetMessagesAsync<Message>(Sam, 10, ct: TestContext.Current.CancellationToken))
            .Select(e => e.Item.Timestamp.UtcTicks);
        Assert.Equal(new long[] { 3000, 2000, 1000 }, ticks);
    }

    [Fact]
    public async Task Paging_with_cursor_skips_nothing_even_with_identical_timestamps()
    {
        for (int i = 0; i < 5; i++)
            await Store(Msg(Sam, Me, ticks: 1000));   // all the same tick

        var seen = new List<long>();
        ConversationCursor? cursor = null;
        while (true)
        {
            var page = await _repo.GetMessagesAsync<Message>(Sam, limit: 2, before: cursor, ct: TestContext.Current.CancellationToken);
            if (page.Count == 0) break;
            seen.AddRange(page.Select(e => e.LocalId));
            cursor = ConversationCursor.From(page[^1]);
        }

        Assert.Equal(5, seen.Count);
        Assert.Equal(5, seen.Distinct().Count());
    }
}
