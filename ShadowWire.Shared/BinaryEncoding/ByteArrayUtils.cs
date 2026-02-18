using ShadowWire.Shared.Protocol;
using System.Buffers;

namespace ShadowWire.Shared.BinaryEncoding;

public static class ByteArrayUtils
{
    /// <summary>
    /// Prepends the <see cref="MessageKind"/> byte to <paramref name="payload"/>.
    /// </summary>
    /// <param name="messageKind">The message kind to prepend.</param>
    /// <param name="payload">The payload bytes to follow the message kind.</param>
    /// <param name="length">The length of the packet (1 + payload length).</param>
    /// <returns>
    /// A buffer containing the message kind followed by the payload.<br/>
    /// <b>Important:</b> Caller must return this buffer to <see cref="ArrayPool{Byte}"/> when done.
    /// </returns>
    /// <remarks>
    /// Only the first <paramref name="length"/> bytes are valid. The rented array may be larger.
    /// </remarks>
    public static byte[] PrependMessageKind(MessageKind messageKind, ReadOnlySpan<byte> payload, out int length)
    {
        length = payload.Length + 1; // Actual length
        byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

        buffer[0] = (byte)messageKind;
        payload.CopyTo(buffer.AsSpan(1));

        return buffer;
    }
}
