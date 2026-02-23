namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct Acknowledge : IEncodable
{
    private const MessageKind MESSAGE_KIND = MessageKind.Acknowledge;


    public byte[] Encode()
        => new byte[] { (byte)MESSAGE_KIND };
}

