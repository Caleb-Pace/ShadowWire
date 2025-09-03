using System.Net.WebSockets;

namespace ShadowWire.Desktop.Client.Network
{
    internal class Connection
    {
        private ClientWebSocket ws = new();


        public Connection(string serverUri)
        {
            const string SUB_PROTOCOL = "sw";
            ws.Options.AddSubProtocol(SUB_PROTOCOL);

            try
            {
                ws.ConnectAsync(new Uri(serverUri), CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (WebSocketException ex)
            {
                // TODO: Implement logging

                Console.WriteLine($"Failed to connect! \"{ex.Message}\""); // TODO: Remove, for debugging
            }
        }

        ~Connection()
        {
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).GetAwaiter().GetResult();
        }

        // TODO: Implement composite message handling
        public async Task SendAsync(byte[] bin)
        {
            if (ws.State != WebSocketState.Open) return; // Early exit

            try
            {
                // TODO: Remove, for debugging
                await ws.SendAsync(new ArraySegment<byte>(bin), WebSocketMessageType.Text, true, CancellationToken.None);
                //await ws.SendAsync(new ArraySegment<byte>(bin), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                // TODO: Implement logging

                Console.WriteLine($"An exception occured! \"{ex.Message}\""); // TODO: Remove, for debugging
            }
        }

        // TODO: Implement proper receiving (continuous)
        // TODO: Implement composite message handling
        public async Task<byte[]> ReceiveAsync()
        {
            if (ws.State != WebSocketState.Open) return []; // Early exit

            try
            {
                var buffer = new byte[1024 * 4];
                var response = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                byte[] respBin = new byte[response.Count];
                Array.Copy(buffer, respBin, response.Count);

                return respBin;
            }
            catch (WebSocketException ex)
            {
                // TODO: Implement logging

                Console.WriteLine($"An exception occured! \"{ex.Message}\""); // TODO: Remove, for debugging
            }

            return []; // Receive failed
        }
    }
}
