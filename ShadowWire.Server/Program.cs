using ShadowWire.Server.Network;

namespace ShadowWire.Server;

internal class Program
{
    static void Main(string[] args)
    {
        RelayServer relayServer = new();
        relayServer.StartAsync().GetAwaiter().GetResult();
    }
}
