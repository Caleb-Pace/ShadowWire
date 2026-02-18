using ShadowWire.Shared.Protocol;

namespace ShadowWire.Server.Handlers;

public sealed class ServerMessageHandlerRegistry : MessageHandlerRegistry
{
    public static readonly ServerMessageHandlerRegistry Instance = new();

    private static readonly Dictionary<byte, IMessageHandlerAdapter> _handlers = new() { };


    private ServerMessageHandlerRegistry() : base(_handlers) { }
}
