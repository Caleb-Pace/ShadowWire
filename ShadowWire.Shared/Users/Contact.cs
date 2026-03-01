using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Users;

public readonly struct Contact
{
    public const int MINIMUM_SIZE = 1 + Fingerprint.SIZE + (2 * sizeof(Int32));

    public readonly string Nickname { get; init; }
    public Fingerprint Fingerprint { get; init; }
    public ReadOnlyMemory<byte> PublicKeyDer { get; init; }


    public Contact(string nickname, Fingerprint fingerprint, byte[] publicKeyDer)
    {
        Nickname = nickname;
        Fingerprint = fingerprint;
        PublicKeyDer = publicKeyDer;
    }

    /// <exception cref="ArgumentException">Thrown if the byte span cannot be decoded.</exception>
    public Contact(ReadOnlySpan<byte> contactBytes)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(contactBytes.Length, MINIMUM_SIZE, nameof(contactBytes));

        var reader = new SpanReader(contactBytes);

        Nickname = reader.ReadString();
        Fingerprint = new Fingerprint(reader.ReadBytes().ToArray());
        PublicKeyDer = reader.ReadBytes().ToArray();
    }

    public byte[] Encode()
    {
        var nicknameSizeInBytes = SpanWriter.GetStringSize(Nickname);
        var length = nicknameSizeInBytes
                   + Fingerprint.SIZE
                   + PublicKeyDer.Length
                   + (3 * sizeof(Int32));

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteString(Nickname, nicknameSizeInBytes);
        writer.WriteBytes(Fingerprint.Span);
        writer.WriteBytes(PublicKeyDer.Span);

        return buffer;
    }
}
