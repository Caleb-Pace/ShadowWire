using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct BadRequest : IEncodable
{
    private const MessageKind MESSAGE_KIND = MessageKind.BadRequest;

    public readonly string message;


    public BadRequest(string message)
        => this.message = message;

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public BadRequest(ReadOnlySpan<byte> messageBytes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, 2, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind
        var reader = new SpanReader(payloadBytes);

        this.message = reader.ReadString();
    }

    public byte[] Encode()
    {
        var length = 1
                   + sizeof(Int32)
                   + message.Length;

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.Write<byte>((byte)MESSAGE_KIND);
        writer.WriteString(message);

        return buffer;
    }
}
