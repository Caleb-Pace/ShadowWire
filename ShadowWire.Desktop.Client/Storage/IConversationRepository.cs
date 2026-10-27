using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Users;

namespace ShadowWire.Desktop.Client.Storage;

public interface IConversationRepository
{
    /// <summary>
    /// Creates the database schema if it does not already exist.
    /// </summary>
    Task InitializeAsync(CancellationToken ct = default);

    /// <summary>
    /// Persists an item as received.
    /// </summary>
    /// <param name="item">The decoded item, used for the queryable columns.</param>
    /// <param name="raw">The original bytes, stored as-is.</param>
    Task StoreAsync<T>(T item, ReadOnlyMemory<byte> raw, CancellationToken ct = default)
        where T : struct, IConversationStorable<T>;

    /// <summary>
    /// Gets a page of items exchanged with a peer, newest first.
    /// </summary>
    /// <param name="peer">The fingerprint of the other party.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="before">If set, only items with an earlier timestamp are returned (for paging).</param>
    Task<IReadOnlyList<StoredEntry<T>>> GetMessagesAsync<T>(
        Fingerprint peer,
        int limit,
        DateTimeOffset? before = null,
        CancellationToken ct = default
    )
        where T : struct, IConversationStorable<T>;

    /// <summary>
    /// Gets unread incoming items, oldest first.
    /// </summary>
    /// <param name="from">If set, only items from this sender are returned.</param>
    Task<IReadOnlyList<StoredEntry<T>>> GetUnreadAsync<T>(
        Fingerprint? from = null,
        CancellationToken ct = default
    )
        where T : struct, IConversationStorable<T>;

    /// <summary>
    /// Marks all incoming items from a peer as read, up to and including a timestamp.
    /// </summary>
    Task MarkReadAsync(Fingerprint peer, DateTimeOffset upTo, CancellationToken ct = default);
}
