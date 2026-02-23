using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;

namespace ShadowWire.Server.Handlers;

public delegate Task SendToAsync(byte[] destFingerprint, IEncodable message, CancellationToken cancellationToken);

public class MessageHandler : IMessageHandler<ClientSession, Message>
{
    private static MessageHandler? _instance;
    private readonly SendToAsync _sendTo;

    public static MessageHandler Instance
        => _instance ?? throw new InvalidOperationException("Not initialized!");


    private MessageHandler(SendToAsync sendTo)
        => _sendTo = sendTo;

    public static void Initialize(SendToAsync sendTo)
    {
        if (_instance != null)
            throw new InvalidOperationException("Already initialized!");

        _instance = new MessageHandler(sendTo);
    }

    public async Task<IEncodable?> HandleAsync(ClientSession context, Message request)
    {
        await _sendTo.Invoke(request.destFingerprint.ToArray(), request, CancellationToken.None);
        return new Acknowledge();
    }
}
