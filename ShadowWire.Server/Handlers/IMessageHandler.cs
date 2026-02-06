using ShadowWire.Server.Network;

namespace ShadowWire.Server.Handlers;

public interface IMessageHandler
{
    Task HandleAsync(SessionHandler session, byte[] messageInBytes);
}
