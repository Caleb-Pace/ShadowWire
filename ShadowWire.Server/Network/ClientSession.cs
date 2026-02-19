using ShadowWire.Server.Handlers;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Users;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

public class ClientSession(WebSocket webSocket, ClientSessionConfig config)
{
    public Guid Id { get; private set; } = Guid.NewGuid(); // Create ID for new conneciton
    public WebSocket WebSocket { get; private set; } = webSocket;

    public Contact? ClientIdentity { get; private set; } = null;

    private readonly ClientSessionConfig sessionConfig = config;


    public async Task<IEncodable?> ReceiveMessageAsync(byte[] buffer)
    {
        var messageKind = buffer[0];
        var messageHandler = ServerMessageHandlerRegistry.Instance.Get(messageKind);
        if (messageHandler == null)
            return null; // Unmapped/unsupported message kind

        return await messageHandler(this, buffer);
    }

    public void AssignClientIdentity(Contact identity)
    {
        ClientIdentity = identity;
        sessionConfig.OnFingerprintChanged(Id, identity.Fingerprint);
    }
}
