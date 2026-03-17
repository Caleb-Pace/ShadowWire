using ShadowWire.Shared.Protocol;

namespace ShadowWire.Shared.BinaryEncoding;

public static class ByteArrayUtils
{
    public static byte[] PrependMessageKind(MessageKind messageKind, ReadOnlySpan<byte> payload)
    {
        byte[] buffer = new byte[payload.Length + 1];

        buffer[0] = (byte)messageKind;
        payload.CopyTo(buffer.AsSpan(1));

        return buffer;
    }
}
