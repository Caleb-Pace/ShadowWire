using ShadowWire.Shared.BinaryEncoding;

namespace ShadowWire.Shared.Protocols.ErrorProtocol;

public class ErrorProtocol : IProtocol<ErrorPacket>
{
    public ProtocolId Protocol => ProtocolId.Error;


    public ErrorPacket Decode(ReadOnlySpan<byte> packetBinary)
    {
        var innerPacket = ProtocolHeader.RemoveProtocolHeader(packetBinary);
        var reader = new SpanReader(innerPacket);

        var errorCode = reader.Read<int>();
        var message = reader.ReadString();

        return new ErrorPacket(errorCode, message);
    }

    public ReadOnlySpan<byte> Encode(ErrorPacket packetStruct)
    {
        var length = sizeof(int) // Error Code
                   + sizeof(Int32) // Message Length
                   + packetStruct.Message.Length; // Message content

        var innerPacketBuffer = new byte[length];
        var writer = new SpanWriter(new Span<byte>(innerPacketBuffer));

        writer.Write<int>(packetStruct.ErrorCode);
        writer.WriteString(packetStruct.Message);

        return ProtocolHeader.PrependProtocolHeader(innerPacketBuffer, Protocol);
    }
}
