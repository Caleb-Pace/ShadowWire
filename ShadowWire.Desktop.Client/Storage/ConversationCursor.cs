using ShadowWire.Shared.Protocol;

namespace ShadowWire.Desktop.Client.Storage;

/// <summary>
/// Marks a position in a conversation, so the next page can start after it.
/// </summary>
/// <remarks>
/// Create one with <see cref="From{T}"/> from the last entry of a page.
/// </remarks>
public readonly record struct ConversationCursor(long Ticks, long LocalId)
{
    /// <summary>
    /// Creates a cursor at the position of the given entry.
    /// </summary>
    public static ConversationCursor From<T>(StoredEntry<T> entry)
        where T : struct, IConversationStorable<T>
        => new(entry.Item.Timestamp.UtcTicks, entry.LocalId);
}
