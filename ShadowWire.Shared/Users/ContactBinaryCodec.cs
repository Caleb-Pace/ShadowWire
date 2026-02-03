using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Users;

/// <summary>
/// Provides methods to encode and decode <see cref="Contact"/> to and from a compact binary format.
/// </summary>
public static class ContactBinaryCodec
{
    /// <summary>
    /// Encodes a <see cref="Contact"/> into a byte span.
    /// </summary>
    /// <param name="contact">The <see cref="Contact"/> to encode.</param>
    /// <returns>A <see cref="Span{Byte}"/> containing the encoded contact.</returns>
    public static Span<byte> Encode(Contact contact)
    {
        var length = contact.Nickname.Length
                   + contact.Fingerprint.Length
                   + contact.PublicKeyDer.Length
                   + (3 * sizeof(Int32));

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteString(contact.Nickname);
        writer.WriteBytes(contact.Fingerprint);
        writer.WriteBytes(contact.PublicKeyDer);

        return buffer;
    }

    /// <summary>
    /// Attempts to decode a <see cref="Contact"/> from a binary array.
    /// </summary>
    /// <param name="serializedContact">The byte span to decode.</param>
    /// <param name="contact">
    /// Output parameter: the decoded contact if successful; otherwise <c>default</c>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if decoding was successful; <see langword="false"/> if the input was invalid or corrupted.
    /// </returns>
    public static bool TryDecode(ReadOnlySpan<byte> contactBinary, out Contact contact)
    {
        try
        {
            var reader = new SpanReader(contactBinary);

            var nickname = reader.ReadString();
            var fingerprint = reader.ReadBytes().ToArray();
            var publicKeyDer = reader.ReadBytes().ToArray();

            contact = new Contact(nickname, fingerprint, publicKeyDer);
            return true;
        }
        catch
        {
            contact = default; // Invalid
            return false;
        }
    }
}
