using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocols.IdentificationProtocol;

public class IdentificationProtocol : IProtocol<Contact>
{
    public ProtocolId Protocol => ProtocolId.Identification;


    public Contact Decode(ReadOnlySpan<byte> packetBinary)
    {
        if (ContactBinaryCodec.TryDecode(packetBinary, out Contact contact))
            return contact;

        throw new ArgumentException("Invalid identification packet!", nameof(packetBinary));
    }

    public ReadOnlySpan<byte> Encode(Contact packetStruct)
    {
        var innerPacket = ContactBinaryCodec.Encode(packetStruct);
        return ProtocolHeader.PrependProtocolHeader(innerPacket, Protocol);
    }
}
