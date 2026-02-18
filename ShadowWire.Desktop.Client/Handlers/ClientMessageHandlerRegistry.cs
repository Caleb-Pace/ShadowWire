using ShadowWire.Shared.Protocol;

namespace ShadowWire.Desktop.Client.Handlers;

public sealed class ClientMessageHandlerRegistry : MessageHandlerRegistry<ClientContext>
{
    public static readonly ClientMessageHandlerRegistry Instance = new();

    private static readonly Dictionary<byte, MessageAdapter<ClientContext>> _handlers = new() { };


    private ClientMessageHandlerRegistry() : base(_handlers) { }
}
