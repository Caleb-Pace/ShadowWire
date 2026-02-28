using System.Buffers.Binary;
using System.Text;

namespace ShadowWire.Shared.BinaryEncoding;

/// <summary>
/// Write sequential data to a <see cref="Span{byte}"/>.
/// </summary>
/// <param name="span">
/// The target span to write into.<br/>
/// <br/>
/// <b>Ensure</b> <see cref="span"/> has sufficient capacity.
/// </param>
/// <remarks>
/// Writes advance an internal position.<br/>
/// <br/>
/// Little-Endian byte ordering is used.<br/>
/// <br/>
/// <see langword="ref struct"/>: cannot be boxed or stored on the heap.
/// </remarks>
public ref struct SpanWriter(Span<byte> span)
{
    private static Encoder utf8Encoder = Encoding.UTF8.GetEncoder();

    private Span<byte> _span = span;
    private int _pos = 0;

    /// <summary>
    /// Current write position in bytes.
    /// </summary>
    public readonly int Position => _pos;


    /// <summary>
    /// Ensures that <paramref name="newByteCount"/> bytes can be written at the current position.
    /// </summary>
    /// <param name="newByteCount">Number of bytes to write.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="newByteCount"/> exceeds the remaining span capacity.
    /// </exception>
    public readonly void EnsureSpace(int newByteCount)
    {
        if ((uint)newByteCount > (uint)(_span.Length - _pos))
            throw new ArgumentOutOfRangeException(nameof(newByteCount), "Attempted to write past the end of the span.");
    }

    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteByte(byte val)
    {
        EnsureSpace(sizeof(byte));
        _span[_pos++] = val;
    }

    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteInt32(Int32 val)
    {
        EnsureSpace(sizeof(Int32));

        BinaryPrimitives.WriteInt32LittleEndian(_span.Slice(_pos), val);
        _pos += sizeof(Int32);
    }

    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteUInt64(UInt64 val)
    {
        EnsureSpace(sizeof(UInt64));

        BinaryPrimitives.WriteUInt64LittleEndian(_span.Slice(_pos), val);
        _pos += sizeof(UInt64);
    }

    /// <summary>
    /// Writes a byte array without a length prefix.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteBytesNoPrefix(ReadOnlySpan<byte> bytes)
    {
        int length = bytes.Length;
        EnsureSpace(length);

        bytes.CopyTo(_span.Slice(_pos));
        _pos += length;
    }

    /// <summary>
    /// Writes a byte array with a 4-byte <see cref="Int32"/> length prefix.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        WriteInt32(bytes.Length);
        WriteBytesNoPrefix(bytes);
    }

    /// <returns>The UTF-8 byte count of <paramref name="str"/>.</returns>
    public static int GetStringSize(string str)
        => Encoding.UTF8.GetByteCount(str);

    /// <summary>
    /// Writes a UTF-8 string with a 4-byte <see cref="Int32"/> length prefix.
    /// </summary>
    /// <param name="str">The string to write.</param>
    /// <param name="byteCount">The exact number of UTF-8 bytes to write.</param>
    /// <remarks>
    /// This overload allows writing a string without recomputing its UTF-8 byte count.<br/>
    /// <br/>
    /// It is the <b>caller's responsibility</b> to ensure that <paramref name="byteCount"/> accurately<br/>
    /// reflects the UTF-8 encoding of <paramref name="str"/>.<br/>
    /// An incorrect <paramref name="byteCount"/> may result in truncated or corrupted output.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteString(string str, int byteCount)
    {
        WriteInt32(byteCount);
        EnsureSpace(byteCount);

        utf8Encoder.GetBytes(str, _span.Slice(_pos, byteCount), flush: true);
        _pos += byteCount;
    }

    /// <summary>
    /// Writes a UTF-8 string with a 4-byte <see cref="Int32"/> length prefix.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteString(string str)
    {
        int byteCount = GetStringSize(str);
        WriteInt32(byteCount);
        EnsureSpace(byteCount);

        Encoding.UTF8.GetBytes(str, _span.Slice(_pos));
        _pos += byteCount;
    }
}
