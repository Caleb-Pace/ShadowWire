using ShadowWire.Server.Network;

namespace ShadowWire.Server.Handlers;

public interface IMessageHandler
{
    Task HandleAsync(ClientSession session, byte[] messageInBytes);
}
