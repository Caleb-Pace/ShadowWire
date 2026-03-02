using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct AuthenticationRequest : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.AuthenticationRequest;

    public readonly Version version;
    public readonly Contact contact;


    public AuthenticationRequest(Version version, Contact contact)
    {
        this.version = version;
        this.contact = contact;
    }

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public AuthenticationRequest(ReadOnlySpan<byte> messageBytes)
    {
        const int MINIMUM_LENGTH = 1
                                 + Version.SIZE
                                 + Contact.MINIMUM_SIZE;

        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, MINIMUM_LENGTH, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind
        var reader = new SpanReader(payloadBytes);

        this.version = new Version(reader.ReadUInt64());
        this.contact = new Contact(payloadBytes[reader.Position..]);
    }

    public byte[] Encode()
    {
        var length = 1
                   + Version.SIZE
                   + contact.GetSize(out int nicknameSizeInBytes);

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteByte((byte)MESSAGE_KIND);
        writer.WriteUInt64(version.Packed);
        contact.Encode(writer, nicknameSizeInBytes);

        return buffer;
    }
}