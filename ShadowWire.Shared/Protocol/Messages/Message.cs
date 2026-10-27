using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public readonly struct Message : IEncodable, IProtocolMessage, IConversationStorable<Message>
{
    private const MessageKind MESSAGE_KIND = MessageKind.Message;
    public static MessageKind Kind => MESSAGE_KIND;

    public Fingerprint Destination => _destFingerprint;
    private readonly Fingerprint _destFingerprint;

    public Fingerprint Source => _srcFingerprint;
    private readonly Fingerprint _srcFingerprint;

    public DateTimeOffset Timestamp => _timestamp;
    private readonly DateTimeOffset _timestamp;
    private const int TIMESTAMP_FIELD_SIZE = sizeof(UInt64);

    public ReadOnlyMemory<byte> Data => _data;
    public readonly ReadOnlyMemory<byte> _data;
    private const int DATA_LENGTH_FIELD_SIZE = sizeof(Int32);


    public Message(Fingerprint destFingerprint, Fingerprint srcFingerprint, DateTimeOffset timestamp, byte[] data)
    {
        this._destFingerprint = destFingerprint;
        this._srcFingerprint  = srcFingerprint;
        this._timestamp       = timestamp;
        this._data            = data;
    }

    /// <exception cref="ArgumentOutOfRangeException">Thrown if the byte span cannot be decoded.</exception>
    public Message(ReadOnlySpan<byte> messageBytes)
    {
        const int MINIMUM_LENGTH = 1
                                 + (2 * Fingerprint.SIZE)
                                 + TIMESTAMP_FIELD_SIZE
                                 + DATA_LENGTH_FIELD_SIZE;

        ArgumentOutOfRangeException.ThrowIfLessThan(messageBytes.Length, MINIMUM_LENGTH, nameof(messageBytes));

        ReadOnlySpan<byte> payloadBytes = messageBytes[1..]; // Skip message kind
        var reader = new SpanReader(payloadBytes);

        this._destFingerprint = Fingerprint.FromSpan( reader.ReadBytes(Fingerprint.SIZE) );

        this._srcFingerprint = Fingerprint.FromSpan( reader.ReadBytes(Fingerprint.SIZE) );

        this._timestamp = reader.ReadDateTimeOffset(convertToLocalTime: true);

        this._data = reader.ReadBytes().ToArray();
    }
    public static Message Decode(ReadOnlySpan<byte> raw)
        => new Message(raw);

    public byte[] Encode()
    {
        var length = 1
                   + (2 * Fingerprint.SIZE)
                   + TIMESTAMP_FIELD_SIZE
                   + DATA_LENGTH_FIELD_SIZE
                   + _data.Length;

        Console.WriteLine($"Message.Encode() : {length} ?= {1 + _destFingerprint.Span.Length + _srcFingerprint.Span.Length + sizeof(UInt64) + 4 + _data.Span.Length}");

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteByte((byte)MESSAGE_KIND);
        writer.WriteBytesNoPrefix(_destFingerprint.Span);
        writer.WriteBytesNoPrefix(_srcFingerprint.Span);
        writer.WriteDateTimeOffset(_timestamp);
        writer.WriteBytes(_data.Span);

        return buffer;
    }
}
