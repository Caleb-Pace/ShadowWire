namespace ShadowWire.Shared.Protocols;

public class ProtocolIdentifier
{
    public static ProtocolId? Identify(ReadOnlySpan<byte> binary)
    {
        if (binary.IsEmpty)
            return null; // Early exit: no binary

        byte idByte = binary[0];

        if (Enum.IsDefined(typeof(ProtocolId), idByte))
            return (ProtocolId)idByte;
        
        // TODO: Implement a log for unknown protcol
        return null; // Unknown protocol
    }
}
