using ShadowWire.Server.Handlers;
using ShadowWire.Shared.Users;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

public class ClientSession(WebSocket webSocket, ClientSessionConfig config)
{
    public Guid Id { get; private set; } = Guid.NewGuid(); // Create ID for new conneciton
    public WebSocket WebSocket { get; private set; } = webSocket;

    public Contact? ClientIdentity { get; private set; } = null;

    private readonly Func<byte[], byte[], Task> _routeMessageAsync = config.routeMessageAsync; // (Fingerprint, Binary)
    private readonly ContactManager _userRegistry = config.userRegistry;


    public async Task ReceiveMessageAsync(byte[] buffer)
    {
        var messageKind = buffer[0];
        var messageHandler = MessageHandlerRegistry.Get(messageKind);
        // TODO: Implement logging - Unknown message kind
        if (messageHandler == null)
            return; // Unmapped/unsupported message kind

        await messageHandler.HandleAsync(this, buffer);

    }

    public void IdentifyClient(Contact identity)
        => ClientIdentity = identity;
}
