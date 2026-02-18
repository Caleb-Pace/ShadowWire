using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct AuthenticationRequest : IEncodable
{
    private const MessageKind MESSAGE_KIND = MessageKind.AuthenticationRequest;

    public readonly Contact contact;


    public AuthenticationRequest(Contact contact)
        => this.contact = contact;

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public AuthenticationRequest(ReadOnlySpan<byte> packetBytes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(packetBytes.Length, 2, nameof(packetBytes));

        ReadOnlySpan<byte> payloadBytes = packetBytes[1..]; // Skip message kind
        if (!ContactBinaryCodec.TryDecode(payloadBytes, out var contact))
            throw new ArgumentException("Invalid contact!");

        this.contact = contact;
    }

    public byte[] Encode()
        => ByteArrayUtils.PrependMessageKind(MESSAGE_KIND, ContactBinaryCodec.Encode(contact));
}
