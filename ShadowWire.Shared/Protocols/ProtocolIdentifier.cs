using System.Linq;

namespace ShadowWire.Shared.Protocols;

public class ProtocolIdentifier
{
    public static ProtocolId? Identify(ReadOnlySpan<byte> binary)
    {
        if (binary.IsEmpty)
            return null; // Early exit: no binary

        // Identify protocol
        byte idByte = binary[0];
        switch (idByte)
        {
            case (byte)ProtocolId.Acknowledge:
            case (byte)ProtocolId.Error:
            case (byte)ProtocolId.Identification:
            case (byte)ProtocolId.Lookup:
            case (byte)ProtocolId.Message:
            case (byte)ProtocolId.OfflineMessagesRequest:
                return (ProtocolId)idByte;
            default:
                // TODO: Implement a log for unknown protcol
                return null; // Unknown protocol
        }
    }
}
