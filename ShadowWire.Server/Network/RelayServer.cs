using ShadowWire.Shared.Users;
using System.Net;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

internal class RelayServer(ContactManager userRegistry)
{
    private readonly SessionManager _sessionManager = new();
    private readonly ContactManager _userRegistry = userRegistry;


    public async Task StartAsync()
    {
        const string URI = "http://127.0.0.1:4960/ws/";
        const string SUB_PROTOCOL = "sw";

        var listener = new HttpListener();
        listener.Prefixes.Add(URI);
        listener.Start();

        // TODO: Remove, for debugging
        Console.WriteLine($"WebSocket server started on \"{URI}\"!");

        var clientSessionConfig = new ClientSessionConfig();

        while (true)
        {
            var context = await listener.GetContextAsync(); // Wait for incoming requests

            if (context.Request.IsWebSocketRequest)
            {
                try
                {
                    var socket = context.Request.RemoteEndPoint;

                    var wsContext = await context.AcceptWebSocketAsync(SUB_PROTOCOL);
                    var session = new ClientSession(wsContext.WebSocket, clientSessionConfig);
                    _sessionManager.TryAdd(session);

                    // TODO: Implement logging
                    Console.WriteLine($"<{session.Id}> connected to {wsContext.RequestUri}! (Socket: {socket.Address}:{socket.Port})"); // TODO: Remove, for debugging

                    HandleConnection(session);
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"An Excecption Occured: \"{ex.Message}\""); // TODO: Remove, for debugging

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

    // TODO: later - add timeout cancellation system
    private async void HandleConnection(ClientSession session)
    {
        var buffer = new byte[1024 * 4]; // 4 MB
        WebSocket ws = session.WebSocket;

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;
                if (result.MessageType == WebSocketMessageType.Binary)
                    await session.ReceiveMessageAsync(buffer);
            }
        }
        catch (Exception ex)
        {
            // TODO: Implement logging
        }
        finally
        {
            _sessionManager.TryRemove(session);
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

            // TODO: Implement logging
            Console.WriteLine($"<{session.Id}> disconnected!"); // TODO: Remove, for debugging
        }
    }

    private async Task RouteMessageAsync(byte[] destFingerprint, byte[] messageBinary)
    {

    }
}
