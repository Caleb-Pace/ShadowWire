using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;
using ShadowWire.Shared.Users;
using System.Net;
using System.Net.WebSockets;

namespace ShadowWire.Server.Network;

internal class RelayServer
{
    private const int BUFFER_SIZE = 4 * 1024; // 4 MB

    private readonly SessionManager _sessionManager;
    private readonly ClientSessionConfig _clientSessionConfig;


    public RelayServer()
    {
        _sessionManager = new();
        _clientSessionConfig = new ClientSessionConfig(_sessionManager.SetFingerprint);
    }

    public async Task InitializeClientSessionAsync(HttpListenerWebSocketContext webSocketContext, IPEndPoint socket)
    {
        var session = new ClientSession(webSocketContext.WebSocket, _clientSessionConfig);
        _sessionManager.TryAdd(session);

        // TODO: Implement logging
        Console.WriteLine($"<{session.Id}> connected to {webSocketContext.RequestUri}! (Socket: {socket.Address}:{socket.Port})"); // TODO: Remove, for debugging

        OnClientConnected(session);
    }

    public async Task HandleWebSocketRequestAsync(HttpListenerContext context, string subProtocol)
    {
        try
        {
            var wsContext = await context.AcceptWebSocketAsync(subProtocol);
            var socket = context.Request.RemoteEndPoint;

            await InitializeClientSessionAsync(wsContext, socket);
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"An Excecption Occured: \"{ex.Message}\""); // TODO: Remove, for debugging

            // TODO: Implement logging

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
        }
    }
    
    private static void HandleNonWebSocketRequest(HttpListenerContext context)
    {
        // TODO: Implement logging

        context.Response.Close(); // Ignore other requests
    }

    public async Task AcceptConnectionsAsync(HttpListener listener, string subProtocol)
    {
        while (true)
        {
            var context = await listener.GetContextAsync(); // Wait for incoming requests

            if (context.Request.IsWebSocketRequest)
                await HandleWebSocketRequestAsync(context, subProtocol);
            else
                HandleNonWebSocketRequest(context);
        }
    }

    public async Task StartAsync()
    {
        const string URI = "http://127.0.0.1:4960/ws/";
        const string SUB_PROTOCOL = "sw";

        var listener = new HttpListener();
        listener.Prefixes.Add(URI);
        listener.Start();

        // TODO: Remove, for debugging
        Console.WriteLine($"WebSocket server started on \"{URI}\"!");

        await AcceptConnectionsAsync(listener, SUB_PROTOCOL);
    }

    private static async Task<IEncodable?> GetResponseAsync(ClientSession session, byte[] buffer)
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

    private static async Task SendResponseAsync(WebSocket ws, IEncodable response)
    {
        var responseBinary = response.Encode();
        ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(responseBinary.Length, BUFFER_SIZE, nameof(responseBinary));
        
        await ws.SendAsync(new ArraySegment<byte>(responseBinary), WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    private static async Task HandleBinaryRequestAsync(ClientSession session, byte[] buffer)
    {
        IEncodable? response = await GetResponseAsync(session, buffer);
        if (response == null)
            return; // No operation

        await SendResponseAsync(session.WebSocket, response);
    }

    private static async Task ServeClientAsync(ClientSession session)
    {
        var buffer = new byte[BUFFER_SIZE]; // 4 MB
        WebSocket ws = session.WebSocket;

        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
                break;
            if (result.MessageType == WebSocketMessageType.Binary)
                await HandleBinaryRequestAsync(session, buffer);
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

    public async Task SendToAsync(Fingerprint destFingerprint, byte[] messageBinary, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan<int>(messageBinary.Length, BUFFER_SIZE, nameof(messageBinary));

        if (!_sessionManager.TryGetSessionByFingerprint(destFingerprint, out var session))
        {
            // TODO: Implement message storing system
            return; // Temporary Early exit: User not active
        }

        await session.WebSocket.SendAsync(new ArraySegment<byte>(messageBinary), WebSocketMessageType.Binary, true, cancellationToken);
    }

    public async Task SendToAsync(Fingerprint destFingerprint, IEncodable message, CancellationToken cancellationToken)
        => await SendToAsync(destFingerprint, message.Encode(), cancellationToken);
}
