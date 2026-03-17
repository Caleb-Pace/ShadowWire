namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct AuthenticationFailure : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.AuthenticationFailure;


    public byte[] Encode()
        => new byte[] { (byte)MESSAGE_KIND };
}
