namespace ShadowWire.Shared.Protocols;

public class PacketHeader
{
    /// <summary>
    /// Creates the header for a packet.
    /// </summary>
    /// <param name="protocol">Protocol of the packet.</param>
    /// <returns>The packet header in bytes.</returns>
    public static ReadOnlySpan<byte> CreateHeader(ProtocolId protocol)
    {
        return (ReadOnlySpan<byte>)new byte[] { (byte)protocol };
    }

    /// <summary>
    /// Strips the header from a packet.
    /// </summary>
    /// <param name="packet">The full packet.</param>
    /// <returns>The packet body.</returns>
    public static ReadOnlySpan<byte> RemoveHeader(ReadOnlySpan<byte> packet)
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
