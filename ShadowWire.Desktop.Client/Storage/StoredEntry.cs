using ShadowWire.Shared.Protocol;

namespace ShadowWire.Desktop.Client.Storage;

public readonly struct StoredEntry<T>(long localId, T item, bool isRead)
    where T : struct, IConversationStorable<T>
{
    public long LocalId { get; } = localId;
    public T Item { get; } = item;
    public bool IsRead { get; } = isRead;
}
