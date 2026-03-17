namespace ShadowWire.Shared.Protocol;

/// <summary>
/// Adapter that converts raw message bytes into a strongly-typed request handler method.
/// </summary>
/// <remarks>
/// The adapter is responsible for decoding <paramref name="messageBytes"/> into the correct request object<br/>
/// and returning a delegate that can be invoked to handle the request asynchronously.<br/>
/// <br/>
/// It does <b>not</b> execute the handler itself.
/// </remarks>
public delegate Task<IEncodable> MessageAdapter<TContext>(TContext context, byte[] messageBytes);

/// <summary>
/// Registry of message adapters keyed by message kind.
/// </summary>
/// <remarks>
/// Each entry maps a message kind to a <see cref="MessageAdapter{TContext}"/>,<br/>
/// which converts raw message bytes into a strongly-typed request handler method.
/// </remarks>
public class MessageHandlerRegistry<TContext>(Dictionary<byte, MessageAdapter<TContext>> handlers)
    where TContext : class
{
    private readonly Dictionary<byte, MessageAdapter<TContext>> _handlers = handlers;


    /// <summary>
    /// Retrieves the message adapter for the specified message kind, or <see langword="null"/> if none exists.
    /// </summary>
    public MessageAdapter<TContext>? Get(byte messageKind)
    {
        if (_handlers.TryGetValue(messageKind, out var handler))
            return handler;

        // TODO: Implement logging - Warning: Unmapped message kind
        return null; // Unmapped message kind
    }
}
