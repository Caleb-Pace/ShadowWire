using ShadowWire.Desktop.Client.Network;
using ShadowWire.Shared.Users;
using Version = ShadowWire.Shared.Version;

namespace ShadowWire.Desktop.Client;

public class ClientContext
{
    public Version Version { get; init; }

    public Contact ClientIdentity { get; init; }

    public Connection Connection { get; init; }


    public ClientContext(Version version, Contact clientIdentity, Connection connection)
    {
        this.Version = version;
        this.ClientIdentity = clientIdentity;
        this.Connection = connection;
    }
}
