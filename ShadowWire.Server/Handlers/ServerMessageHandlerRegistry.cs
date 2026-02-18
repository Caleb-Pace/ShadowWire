using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;

namespace ShadowWire.Server.Handlers;

public sealed class ServerMessageHandlerRegistry : MessageHandlerRegistry<ClientSession>
{
    public static readonly ServerMessageHandlerRegistry Instance = new();

    private static readonly Dictionary<byte, MessageAdapter<ClientSession>> _handlers = new() { };


    private ServerMessageHandlerRegistry() : base(_handlers) { }
}
