using ShadowWire.Server.Handlers;
using ShadowWire.Shared.Users;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

public class ClientSession(WebSocket webSocket, Func<byte[], byte[], Task> routeMessageAsync, ContactManager userRegistry)
{
    public Guid Id { get; private set; } = Guid.NewGuid(); // Create ID for new conneciton
    public WebSocket WebSocket { get; private set; } = webSocket;

    private readonly Func<byte[], byte[], Task> _routeMessageAsync = routeMessageAsync; // (Fingerprint, Binary)
    private readonly ContactManager _userRegistry = userRegistry;

    private static Dictionary<byte, IMessageHandler> MessageHandlers => new();


    public async Task ReceiveMessageAsync(byte[] buffer)
    {
        var messageKind = buffer[0];
        if (MessageHandlers.TryGetValue(messageKind, out IMessageHandler messageHandler))
        {
            await messageHandler.HandleAsync(this, buffer);
            return;
        }

        // TODO: Implement logging - Unknown message kind
    }
}
