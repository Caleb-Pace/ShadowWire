using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct BadRequest : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.BadRequest;

    private const int MessageLengthFieldSize = sizeof(Int32);

    public readonly string message;


    public BadRequest(string message)
        => this.message = message;

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public BadRequest(ReadOnlySpan<byte> messageBytes)
    {
        const int MINIMUM_LENGTH = 1 + MessageLengthFieldSize;

        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, MINIMUM_LENGTH, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind
        var reader = new SpanReader(payloadBytes);

        this.message = reader.ReadString();
    }

    public byte[] Encode()
    {
        var messageSizeInBytes = SpanWriter.GetStringSize(message);
        var length = 1
                   + MessageLengthFieldSize
                   + messageSizeInBytes;

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteByte((byte)MESSAGE_KIND);
        writer.WriteString(message, messageSizeInBytes);

        return buffer;
    }
}
