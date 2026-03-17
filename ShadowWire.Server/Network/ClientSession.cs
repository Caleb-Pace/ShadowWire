using ShadowWire.Shared.Users;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

public class ClientSession(WebSocket webSocket, ClientSessionConfig config)
{
    public Guid Id { get; private set; } = Guid.NewGuid(); // Create ID for new conneciton
    public WebSocket WebSocket { get; private set; } = webSocket;

    public Contact? ClientIdentity { get; private set; } = null;

    private readonly ClientSessionConfig sessionConfig = config;


    public void AssignClientIdentity(Contact identity)
    {
        ClientIdentity = identity;
        sessionConfig.OnFingerprintChanged(Id, identity.Fingerprint);
    }
}
