using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public class LookupRequest : IEncodable
{
    private const MessageKind MESSAGE_KIND = MessageKind.LookupRequest;
    private const int REQUEST_LENGTH = 1 + Fingerprint.SIZE;

    public readonly ReadOnlyMemory<byte> fingerprint;


    public LookupRequest(ReadOnlyMemory<byte> fingerprint)
        => this.fingerprint = fingerprint;

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public LookupRequest(ReadOnlySpan<byte> messageBytes)
    {

        ArgumentOutOfRangeException.ThrowIfNotEqual(messageBytes.Length, REQUEST_LENGTH, nameof(messageBytes));

        this.fingerprint = messageBytes[1..].ToArray(); // Skip message kind
    }

    public byte[] Encode()
    {
        var buffer = new byte[REQUEST_LENGTH];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteByte((byte)MESSAGE_KIND);
        writer.WriteBytesNoPrefix(fingerprint.Span);

        return buffer;
    }
}
