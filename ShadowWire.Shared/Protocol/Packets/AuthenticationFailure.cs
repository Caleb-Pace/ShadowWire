namespace ShadowWire.Shared.Protocol.Packets;

public readonly struct AuthenticationFailure : IEncodable
{
    private const MessageKind MESSAGE_KIND = MessageKind.AuthenticationFailure;


    public byte[] Encode()
        => new byte[] { (byte)MESSAGE_KIND };
}
