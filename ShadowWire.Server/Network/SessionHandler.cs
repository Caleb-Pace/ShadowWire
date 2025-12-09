using ShadowWire.Shared.Users;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

internal class SessionHandler(WebSocket webSocket, Func<byte[], byte[], Task> routeMessageAsync, ContactManager userRegistry)
{
    public Guid Id { get; private set; } = Guid.NewGuid(); // Create ID for new conneciton
    public WebSocket WebSocket { get; private set; } = webSocket;

    private readonly Func<byte[], byte[], Task> _routeMessageAsync = routeMessageAsync; // (Fingerprint, Binary)
    private readonly ContactManager _userRegistry = userRegistry;


    public async Task ReceiveMessage(byte[] buffer)
    {
    }
}
