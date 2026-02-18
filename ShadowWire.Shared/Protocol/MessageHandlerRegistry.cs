namespace ShadowWire.Shared.Protocol;

/// <summary>
/// Registry of <see cref="IMessageHandlerAdapter"/> instances keyed by message kind.
/// </summary>
public class MessageHandlerRegistry(Dictionary<byte, IMessageHandlerAdapter> handlers)
{
    private readonly Dictionary<byte, IMessageHandlerAdapter> _handlers = handlers;


    /// <summary>
    /// Retrieves the handler for the specified message kind, or <see langword="null"/> if none exists.
    /// </summary>
    public IMessageHandlerAdapter? Get(byte messageKind)
    {
        if (_handlers.TryGetValue(messageKind, out var handler))
            return handler;

        // TODO: Implement logging - Warning: Unmapped message kind
        return null; // Unmapped message kind
    }
}
