using ShadowWire.Server.Network;
using ShadowWire.Shared.Users;

namespace ShadowWire.Server;

internal class Program
{
    static void Main(string[] args)
    {
        var userRegistry = new ContactManager("registry.bin");

        var relayServer = new RelayServer(userRegistry);
        relayServer.StartAsync().GetAwaiter().GetResult();
    }
}
