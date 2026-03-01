using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public sealed class LookupResponse : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.LookupResponse;

    public readonly Contact contact;


    public LookupResponse(Contact contact)
        => this.contact = contact;

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public LookupResponse(ReadOnlySpan<byte> messageBytes)
    {
        const int MINIMUM_LENGTH = 1
                                 + Contact.MINIMUM_SIZE;

        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, MINIMUM_LENGTH, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind

        this.contact = new Contact(payloadBytes);
    }

    public byte[] Encode()
    {
        var length = 1
                   + contact.GetSize(out int nicknameSizeInBytes);

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteByte((byte)MESSAGE_KIND);
        contact.Encode(writer, nicknameSizeInBytes);

        return buffer;
    }
}