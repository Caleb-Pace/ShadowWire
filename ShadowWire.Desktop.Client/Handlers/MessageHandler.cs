using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;

namespace ShadowWire.Desktop.Client.Handlers;

public class MessageHandler : IMessageHandler<ClientContext, Message>
{
    private static MessageHandler? _instance;
    //private readonly MessageRegistry _messageRegistry;

    public static MessageHandler Instance
        => _instance ?? throw new InvalidOperationException("Not initialized!");


    //private MessageHandler(MessageRegistry messageRegistry)
    //    => _messageRegistry = messageRegistry;

    //public static void Initialize(MessageRegistry messageRegistry)
    //{
    //    if (_instance != null)
    //        throw new InvalidOperationException("Already initialized!");

    //    _instance = new MessageHandler(messageRegistry);
    //}
    public static void Initialize()
    {
        if (_instance != null)
            throw new InvalidOperationException("Already initialized!");

        _instance = new MessageHandler();
    }


    public async Task<IEncodable> HandleAsync(ClientContext context, Message request)
    {
        throw new NotImplementedException();
    }
}
