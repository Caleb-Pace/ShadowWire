namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct AuthenticationSuccess : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.AuthenticationSuccess;


    public byte[] Encode()
        => new byte[] { (byte)MESSAGE_KIND };
}
