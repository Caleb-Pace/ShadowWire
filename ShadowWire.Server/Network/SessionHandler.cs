using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

internal class SessionHandler(WebSocket webSocket)
{
    public Guid Id { get; private set; } = Guid.NewGuid(); // Create ID for new conneciton
    public WebSocket WebSocket { get; private set; } = webSocket;

}
