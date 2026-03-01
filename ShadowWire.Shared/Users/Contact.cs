using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Users;

public readonly struct Contact
{
    private const int NicknameLengthFieldSize = sizeof(Int32);
    private const int PublicKeyLengthFieldSize = sizeof(Int32);

    /// <summary>
    /// The smallest possible valid encoded <see cref="Contact"/> in bytes.
    /// <para>
    /// Consists of a fingerprint and the length fields for the nickname and public key.
    /// </para>
    /// </summary>
    public const int MINIMUM_SIZE = NicknameLengthFieldSize + Fingerprint.SIZE + PublicKeyLengthFieldSize;

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

    /// <summary>
    /// Calculates the exact byte size of this contact when encoded.
    /// </summary>
    /// <param name="nicknameSizeInBytes">Outputs the UTF-8 byte count of <see cref="Nickname"/>.</param>
    public int GetSize(out int nicknameSizeInBytes)
    {
        nicknameSizeInBytes = SpanWriter.GetStringSize(Nickname);

        return NicknameLengthFieldSize
             + nicknameSizeInBytes
             + Fingerprint.SIZE
             + PublicKeyLengthFieldSize
             + PublicKeyDer.Length;
    }

    public void Encode(SpanWriter writer, int? nicknameSizeInBytes = null)
    {
        if (nicknameSizeInBytes.HasValue)
            writer.WriteString(Nickname, nicknameSizeInBytes.Value);
        else
            writer.WriteString(Nickname);

        writer.WriteBytes(Fingerprint.Span);
        writer.WriteBytes(PublicKeyDer.Span);
    }

    public byte[] Encode()
    {
        int length = GetSize(out int nicknameSizeInBytes);

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        Encode(writer, nicknameSizeInBytes);

        return buffer;
    }
}
