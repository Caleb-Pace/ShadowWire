using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;
using ShadowWire.Shared.Users;

namespace ShadowWire.Server.Handlers;

public delegate Task SendToAsync(Fingerprint destFingerprint, IEncodable message, CancellationToken cancellationToken);

// TODO: Set timestamp in HandleAsync.
// TODO: Verify sender fingerprint.
// TODO: (Post-MVP) return sent timestamp instead of ACK so sender can store common timestamp.
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

    public async Task<IEncodable> HandleAsync(ClientSession context, Message request)
    {
        await _sendTo.Invoke(request.Destination, request, CancellationToken.None);
        return new Acknowledge();
    }
}
