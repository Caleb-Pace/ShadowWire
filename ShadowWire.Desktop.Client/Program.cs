using ShadowWire.Desktop.Client.Network;
using System.Text;

namespace ShadowWire.Desktop.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string URI = "ws://127.0.0.1:4960/ws/";

            var connection = new Connection(URI);

            await connection.SendAsync(Encoding.UTF8.GetBytes("Hello"));

            var respBin = await connection.ReceiveAsync();
            Console.WriteLine($"Response from the server: \"{Encoding.UTF8.GetString(respBin)}\"");
        }
    }
}
