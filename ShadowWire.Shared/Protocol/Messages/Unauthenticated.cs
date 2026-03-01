namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct Unauthenticated : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.Unauthenticated;


    public byte[] Encode()
        => new byte[] { (byte)MESSAGE_KIND };
}
