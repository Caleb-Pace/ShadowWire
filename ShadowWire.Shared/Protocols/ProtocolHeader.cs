namespace ShadowWire.Shared.Protocols;

public static class ProtocolHeader
{
    /// <summary>
    /// Prepends the protocol header byte to an existing packet.
    /// </summary>
    /// <param name="innerPacket">The existing protocol-specific packet, without the protocol header.</param>
    /// <param name="protocol">The protocol identifier.</param>
    /// <returns>The completed packet with the protocol header byte.</returns>
    public static byte[] PrependProtocolHeader(ReadOnlySpan<byte> innerPacket, ProtocolId protocol)
    {
        // Allocate a buffer for header + packet
        byte[] buffer = new byte[1 + innerPacket.Length];

        // Set header
        buffer[0] = (byte)protocol;

        // Copy the inner protocol packet after the header
        if (innerPacket.Length > 0)
            innerPacket.CopyTo(buffer.AsSpan(1));

        return buffer;
    }

    /// <summary>
    /// Removes the protocol header byte from a packet.
    /// </summary>
    /// <param name="packet">The full packet.</param>
    /// <returns>The rest of the packet.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="packet"/> is too short to contain a protocol header.
    /// </exception>
    public static ReadOnlySpan<byte> RemoveProtocolHeader(ReadOnlySpan<byte> packet)
    {
        if (packet.Length < 1)
            throw new ArgumentException("Packet is too short to contain a protocol header.", nameof(packet));

        return packet[1..]; // Skip first byte (ID byte)
    }

    /// <summary>
    /// Reads the packet protocol from the header.
    /// </summary>
    /// <param name="packet">The full packet.</param>
    /// <returns>The protocol the packet is using.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="packet"/> is too short to contain a protocol header.
    /// </exception>
    public static ProtocolId? IdentifyProtocol(ReadOnlySpan<byte> packet)
    {
        if (packet.Length < 1)
            throw new ArgumentException("Packet is too short to contain a protocol header.", nameof(packet));

        // Identify protocol
        byte idByte = packet[0];
        ProtocolId? protocol = Enum.IsDefined(typeof(ProtocolId), idByte)
                             ? (ProtocolId)idByte
                             : null;

        // TODO: Implement a log for unknown protocol
        //if (!protocol.HasValue)

        return protocol;
    }
}
