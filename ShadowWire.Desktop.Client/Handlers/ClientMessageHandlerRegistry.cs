using ShadowWire.Shared.Protocol;

namespace ShadowWire.Desktop.Client.Handlers;

public sealed class ClientMessageHandlerRegistry : MessageHandlerRegistry<ClientContext>
{
    private static readonly Dictionary<byte, MessageAdapter<ClientContext>> _handlers = new() { };

    public static readonly ClientMessageHandlerRegistry Instance = new();


    private ClientMessageHandlerRegistry() : base(_handlers) { }
}
