using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct Message : IEncodable
{
    private const MessageKind MESSAGE_KIND = MessageKind.Message;

    public readonly ReadOnlyMemory<byte> destFingerprint;
    public readonly ReadOnlyMemory<byte> data;


    public Message(byte[] destFingerprint, byte[] data)
    {
        this.destFingerprint = destFingerprint;
        this.data = data;
    }

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public Message(ReadOnlySpan<byte> messageBytes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, 2, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind
        var reader = new SpanReader(payloadBytes);

        this.destFingerprint = reader.ReadBytes().ToArray();
        this.data = reader.ReadBytes().ToArray();
    }

    public byte[] Encode()
    {
        var length = 1
                   + destFingerprint.Length
                   + data.Length
                   + (2 * sizeof(Int32));

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.Write<byte>((byte)MESSAGE_KIND);
        writer.WriteBytes(destFingerprint.Span);
        writer.WriteBytes(data.Span);

        return buffer;
    }
}
