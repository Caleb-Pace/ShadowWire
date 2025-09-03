using System.Net.WebSockets;
using System.Text;

namespace ShadowWire.Desktop.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string URI = "ws://127.0.0.1:4960/ws/";
            const string SUB_PROTOCOL = "sw";

            try
            {
                using var ws = new ClientWebSocket();
                ws.Options.AddSubProtocol(SUB_PROTOCOL);
                await ws.ConnectAsync(new(URI), CancellationToken.None);

                // TODO: Remove, for debugging
                var message = Encoding.UTF8.GetBytes("Hello from client!");
                await ws.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);

                // TODO: Remove, for debugging
                var buffer = new byte[1024 * 4];
                var response = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var respMsg = Encoding.UTF8.GetString(buffer, 0, response.Count);
                Console.WriteLine($"Server says: \"{respMsg}\"");

                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                // TODO: Implement logging

                Console.WriteLine($"An error occured! \"{ex.Message}\""); // TODO: Remove, for debugging
            }
        }
    }
}
