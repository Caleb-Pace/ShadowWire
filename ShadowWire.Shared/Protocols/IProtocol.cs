namespace ShadowWire.Shared.Protocols;

/// <typeparam name="TPacketPayload">The payload structure contained in the packet body.</typeparam>
public interface IProtocol<TPacketPayload>
    where TPacketPayload : struct
{
    /// <summary>
    /// The protocol ID as a byte-backed enum.
    /// </summary>
    public ProtocolId Id { get; }


    /// <summary>
    /// Encodes a payload into a protocol packet.
    /// </summary>
    /// <param name="payload">The payload to encode.</param>
    /// <returns>The encoded packet in bytes.</returns>
    public ReadOnlySpan<byte> Encode(TPacketPayload payload);

    /// <summary>
    /// Decodes a protocol packet into its payload.
    /// </summary>
    /// <param name="packetBinary">The full packet in bytes.</param>
    /// <returns>The decoded payload.</returns>
    public TPacketPayload Decode(ReadOnlySpan<byte> packetBinary);
}
