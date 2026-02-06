namespace ShadowWire.Server.Handlers;

/// <summary>
/// Maintains a registry of <see cref="IMessageHandler"/> instances keyed by message kind.
/// </summary>
public static class MessageHandlerRegistry
{
    private static readonly Dictionary<byte, IMessageHandler> _handlers = new() { };


    /// <summary>
    /// Retrieves the <see cref="IMessageHandler"/> associated with the given message kind.
    /// </summary>
    /// <param name="messageKind">The identifier for the message type (first byte of the message).</param>
    /// <returns>
    /// The corresponding <see cref="IMessageHandler"/> if found; otherwise, <see langword="null"/> 
    /// to indicate an unmapped message kind.
    /// </returns>
    public static IMessageHandler? Get(byte messageKind)
    {
        if (_handlers.TryGetValue(messageKind, out var handler))
            return handler;

        return null; // Unmapped message kind
    }
}
