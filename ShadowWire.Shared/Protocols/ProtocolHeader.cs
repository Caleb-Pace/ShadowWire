namespace ShadowWire.Shared.Protocols;

public class ProtocolHeader
{
    /// <summary>
    /// Creates the protocol header byte for the given protocol.
    /// </summary>
    /// <param name="protocol">Protocol of the packet.</param>
    /// <returns>The protocol header byte.</returns>
    public static byte CreateProtocolHeader(ProtocolId protocol)
    {
        return (byte)protocol;
    }

    /// <summary>
    /// Removes the protocol header byte from a packet.
    /// </summary>
    /// <param name="packet">The full packet.</param>
    /// <returns>The rest of the packet.</returns>
    public static ReadOnlySpan<byte> RemoveProtocolHeader(ReadOnlySpan<byte> packet)
    {
        return packet[1..]; // Skip first byte (ID byte)
    }

    // TODO: Change null to exceptions
    /// <summary>
    /// Reads the packet protocol from the header.
    /// </summary>
    /// <param name="packet">The full packet.</param>
    /// <returns>The protocol the packet is using.</returns>
    public static ProtocolId? IdentifyProtocol(ReadOnlySpan<byte> packet)
    {
        if (packet.IsEmpty)
            return null; // Early exit: no binary

        // Identify protocol
        byte idByte = packet[0];
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
                // TODO: Implement a log for unknown protocol
                return null; // Unknown protocol
        }
    }
}
