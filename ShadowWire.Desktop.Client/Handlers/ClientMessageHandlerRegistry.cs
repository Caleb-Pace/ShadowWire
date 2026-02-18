using ShadowWire.Shared.Protocol;

namespace ShadowWire.Desktop.Client.Handlers;

public sealed class ClientMessageHandlerRegistry : MessageHandlerRegistry
{
    public static readonly ClientMessageHandlerRegistry Instance = new();

    private static readonly Dictionary<byte, IMessageHandlerAdapter> _handlers = new() { };


    private ClientMessageHandlerRegistry() : base(_handlers) { }
}
