namespace ShadowWire.Shared.Protocols;

/// <typeparam name="TPacket">The structure representing the packet fields (header + body).</typeparam>
public interface IProtocol<TPacket>
    where TPacket : struct
{
    /// <summary>
    /// The protocol ID as a byte-backed enum.
    /// </summary>
    public ProtocolId Id { get; }


    /// <summary>
    /// Encodes a packet structure into bytes.
    /// </summary>
    /// <param name="packetStruct">The packet structure to encode.</param>
    /// <returns>The encoded packet as bytes.</returns>
    public ReadOnlySpan<byte> Encode(TPacket packetStruct);

    /// <summary>
    /// Decodes a packet from bytes.
    /// </summary>
    /// <param name="packetBinary">The full packet in bytes.</param>
    /// <returns>The decoded packet structure.</returns>
    public TPacket Decode(ReadOnlySpan<byte> packetBinary);
}
