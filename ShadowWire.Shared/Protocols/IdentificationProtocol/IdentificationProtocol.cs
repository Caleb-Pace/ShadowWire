using ShadowWire.Shared.BinaryEncoding;
using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocols.IdentificationProtocol;

public class IdentificationProtocol : IProtocol<Contact>
{
    public ProtocolId Id => ProtocolId.Identification;


    public Contact Decode(ReadOnlySpan<byte> packetBinary)
    {
        var reader = new SpanReader(packetBinary);

        var nickname     = reader.ReadString();
        var fingerprint  = reader.ReadBytes().ToArray();
        var publicKeyDer = reader.ReadBytes().ToArray();

        return new Contact(nickname, fingerprint, publicKeyDer);
    }

    public ReadOnlySpan<byte> Encode(Contact packetStruct)
    {
        var length = packetStruct.Nickname.Length
                   + packetStruct.Fingerprint.Length
                   + packetStruct.PublicKeyDer.Length
                   + (3 * sizeof(Int32));

        var buffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(buffer));

        writer.WriteString(packetStruct.Nickname);
        writer.WriteBytes(packetStruct.Fingerprint);
        writer.WriteBytes(packetStruct.PublicKeyDer);

        return buffer;
    }
}
