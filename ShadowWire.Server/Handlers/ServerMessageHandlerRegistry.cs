using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;

namespace ShadowWire.Server.Handlers;

public sealed class ServerMessageHandlerRegistry : MessageHandlerRegistry<ClientSession>
{
    public static readonly ServerMessageHandlerRegistry Instance = new();

    private static readonly Dictionary<byte, MessageAdapter<ClientSession>> _handlers = new()
    {
        {
            (byte)MessageKind.AuthenticationRequest, (session, messageBytes) =>
                AuthenticationHandler.Instance.HandleAsync(session, new AuthenticationRequest(messageBytes))
        },
        {
            (byte)MessageKind.Message, (session, messageBytes) =>
                MessageHandler.Instance.HandleAsync(session, new Message(messageBytes))
        }
    };



    private ServerMessageHandlerRegistry() : base(_handlers) { }
}
