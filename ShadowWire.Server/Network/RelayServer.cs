using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;
using ShadowWire.Shared.Users;
using System.Net;
using System.Net.WebSockets;
using static System.Collections.Specialized.BitVector32;

namespace ShadowWire.Server.Network;

internal class RelayServer(ContactManager userRegistry)
{
    private readonly SessionManager _sessionManager = new();
    private readonly ContactManager _userRegistry = userRegistry;

    private const int BUFFER_SIZE = 4 * 1024; // 4 MB


    public async Task StartAsync()
    {
        const string URI = "http://127.0.0.1:4960/ws/";
        const string SUB_PROTOCOL = "sw";

        var listener = new HttpListener();
        listener.Prefixes.Add(URI);
        listener.Start();

        // TODO: Remove, for debugging
        Console.WriteLine($"WebSocket server started on \"{URI}\"!");

        var clientSessionConfig = new ClientSessionConfig(_sessionManager.SetFingerprint);

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

                    OnClientConnected(session);
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

    private async Task<IEncodable?> GetResponseAsync(ClientSession session, byte[] buffer)
    {
        try
        {
            return await MessageRouter.ProcessMessageAsync(session, buffer);
        }
        catch (Exception ex)
        {
            return new BadRequest(ex.Message);
        }
    }

    private async Task SendResponseAsync(ClientSession session, IEncodable response)
    {
        var responseBinary = response.Encode();
        ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(responseBinary.Length, BUFFER_SIZE, nameof(responseBinary));
        
        await session.WebSocket.SendAsync(new ArraySegment<byte>(responseBinary), WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    private async Task ServeClientAsync(ClientSession session)
    {
        var buffer = new byte[BUFFER_SIZE]; // 4 MB
        WebSocket ws = session.WebSocket;

        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
                break;
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                IEncodable? response = await GetResponseAsync(session, buffer);
                if (response == null)
                    continue; // No operation

                await SendResponseAsync(session, response);
            }
        }
    }

    // TODO: later - add timeout cancellation system
    private async void OnClientConnected(ClientSession session)
    {
        try
        {
            await ServeClientAsync(session);
        }
        catch (Exception ex)
        {
            // TODO: Implement logging
        }
        finally
        {
            _sessionManager.TryRemove(session);
            await session.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

            // TODO: Implement logging
            Console.WriteLine($"<{session.Id}> disconnected!"); // TODO: Remove, for debugging
        }
    }

    public async Task SendToAsync(byte[] destFingerprint, IEncodable message, CancellationToken cancellationToken)
        => await SendToAsync(destFingerprint, message.Encode(), cancellationToken);

    public async Task SendToAsync(byte[] destFingerprint, byte[] messageBinary, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(messageBinary.Length, BUFFER_SIZE, nameof(messageBinary));

        if (!_sessionManager.TryGetSessionByFingerprint(destFingerprint, out var session))
        {
            // TODO: Implement message storing system
            return; // Temporary Early exit: User not active
        }

        await session.WebSocket.SendAsync(new ArraySegment<byte>(messageBinary), WebSocketMessageType.Binary, true, cancellationToken);
    }
}
