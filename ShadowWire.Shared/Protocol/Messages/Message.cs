using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct Message : IEncodable, IProtocolMessage
{
    private const MessageKind MESSAGE_KIND = MessageKind.Message;

    private const int DataLengthFieldSize = sizeof(Int32);

    public readonly Fingerprint destFingerprint;
    public readonly ReadOnlyMemory<byte> data;


    public Message(Fingerprint destFingerprint, byte[] data)
    {
        this.destFingerprint = destFingerprint;
        this.data = data;
    }

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public Message(ReadOnlySpan<byte> messageBytes)
    {
        const int MINIMUM_LENGTH = 1
                                 + Fingerprint.SIZE
                                 + DataLengthFieldSize;

        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, MINIMUM_LENGTH, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind
        var reader = new SpanReader(payloadBytes);

        var fingerprintBytes = reader.ReadBytes(Fingerprint.SIZE);
        this.destFingerprint = new Fingerprint(fingerprintBytes.ToArray());
        this.data = reader.ReadBytes().ToArray();
    }

    public byte[] Encode()
    {
        var length = 1
                   + Fingerprint.SIZE
                   + DataLengthFieldSize
                   + data.Length;

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteByte((byte)MESSAGE_KIND);
        writer.WriteBytesNoPrefix(destFingerprint.Span);
        writer.WriteBytes(data.Span);

        return buffer;
    }
}
