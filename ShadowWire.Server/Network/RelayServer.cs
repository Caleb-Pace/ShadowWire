using ShadowWire.Shared.Users;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

internal class RelayServer(ContactManager userRegistry)
{
    private ConcurrentDictionary<Guid, ClientSession> _sessions = new();
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

        var clientSessionConfig = new ClientSessionConfig(RouteMessageAsync, _userRegistry);

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
                    _sessions.TryAdd(session.Id, session);

                    // TODO: Implement logging
                    Console.WriteLine($"<{session.Id}> connected to {wsContext.RequestUri}! (Socket: {socket.Address}:{socket.Port})"); // TODO: Remove, for debugging

                    HandleConnection(session.Id);
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

    private async void HandleConnection(Guid sessionId)
    {
        var buffer = new byte[1024 * 4]; // 4 MB
        WebSocket ws = _sessions[sessionId].WebSocket;

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;
                if (result.MessageType == WebSocketMessageType.Binary)
                    await _sessions[sessionId].ReceiveMessageAsync(buffer);
            }
        }
        catch (Exception ex)
        {
            // TODO: Implement logging
        }
        finally
        {
            _sessions.TryRemove(sessionId, out _);
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

            // TODO: Implement logging
            Console.WriteLine($"<{sessionId}> disconnected!"); // TODO: Remove, for debugging
        }
    }

    private async Task RouteMessageAsync(byte[] destFingerprint, byte[] messageBinary)
    {

    }
}
