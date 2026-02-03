using System.Buffers.Binary;
using System.Text;

namespace ShadowWire.Shared.Protocols.ErrorProtocol;

public class ErrorProtocol : IProtocol<ErrorPacket>
{
    public ProtocolId Id => ProtocolId.Error;


    public ErrorPacket Decode(ReadOnlySpan<byte> packetBinary)
    {
        if (packetBinary.IsEmpty || packetBinary.Length <= (sizeof(int) + sizeof(int)))
            throw new InvalidOperationException("Invalid Packet! (Too short)");

        int readPtr = 0; // Packet byte pointer

        // Read error code
        int errorCode = BinaryPrimitives.ReadInt32LittleEndian(packetBinary.Slice(readPtr, sizeof(int)));
        readPtr += sizeof(int);

        // Read message length
        int msgLength = BinaryPrimitives.ReadInt32LittleEndian(packetBinary.Slice(readPtr, sizeof(int)));
        readPtr += sizeof(int);

        if ((readPtr + msgLength) > packetBinary.Length)
            throw new InvalidOperationException("Invalid message length!");

        // Read message
        string message = Encoding.UTF8.GetString(packetBinary.Slice(readPtr, msgLength));

        return new ErrorPacket(errorCode, message);
    }

    public ReadOnlySpan<byte> Encode(ErrorPacket packetStruct)
    {
        int msgLength = Encoding.UTF8.GetByteCount(packetStruct.Message);
        byte[] buffer = new byte[sizeof(int) + sizeof(int) + msgLength];
        int writePtr = 0; // Buffer position pointer

        // Write error code
        BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(writePtr, sizeof(int)), packetStruct.ErrorCode);
        writePtr += sizeof(int);

        // Write message length
        BinaryPrimitives.WriteInt32LittleEndian(buffer.AsSpan(writePtr, sizeof(int)), msgLength);
        writePtr += sizeof(int);

        // Write message bytes
        Encoding.UTF8.GetBytes(packetStruct.Message, buffer.AsSpan(writePtr, msgLength));

        return buffer;
    }
}
