using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace ShadowWire.Server
{
    internal class Program
    {
        private static ConcurrentDictionary<Guid, WebSocket> sessions = new();

        static async Task Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:4960/ws/");
            listener.Start();

            // TODO: Remove, for debugging
            string uri = listener.Prefixes.First();
            Console.WriteLine($"WebSocket server started on \"{uri}\"!");

            while (true)
            {
                var context = await listener.GetContextAsync(); // Wait for incoming requests

                if (context.Request.IsWebSocketRequest)
                {
                    try
                    {
                        var socket = context.Request.RemoteEndPoint;

                        var wsContext = await context.AcceptWebSocketAsync("sw");
                        var sessionId = Guid.NewGuid(); // Create ID for new conneciton
                        sessions.TryAdd(sessionId, wsContext.WebSocket);

                        // TODO: Implement logging
                        Console.WriteLine($"<{sessionId}> connected to {wsContext.RequestUri}! (Socket: {socket.Address}:{socket.Port})"); // TODO: Remove, for debugging

                        HandleConnection(sessionId);
                    }
                    catch (WebSocketException ex)
                    {
                        // TODO: Implement logging

                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
                else
                {
                    // TODO: Implement logging

                    // Ignore other requests
                    context.Response.Close();
                }
            }
        }

        private static async void HandleConnection(Guid sessionId)
        {
            var buffer = new byte[1024 * 4];
            WebSocket ws = sessions[sessionId];

            try
            {
                while (ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // TODO: Remove, for debugging
                        var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"<{sessionId}> sent \"{msg}\" (Echoed back)");

                        await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Implement logging
            }
            finally
            {
                sessions.TryRemove(sessionId, out _);
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

                // TODO: Implement logging
                Console.WriteLine($"<{sessionId}> disconnected!"); // TODO: Remove, for debugging
            }
        }
    }
}
