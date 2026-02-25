using System.Buffers.Binary;
using System.Text;

namespace ShadowWire.Shared.BinaryEncoding;

/// <summary>
/// Write sequential data to a <see cref="Span{byte}"/>.
/// </summary>
/// <param name="span">The target span to write into.</param>
/// <remarks>
/// <b>Ensure</b> <see cref="span"/> is allocated enough memory.<br/>
/// <br/>
/// Writes advance an internal position.<br/>
/// <br/>
/// Little-Endian byte ordering is used.<br/>
/// <br/>
/// <see langword="ref struct"/>: cannot be boxed or stored on the heap.
/// </remarks>
public ref struct SpanWriter(Span<byte> span)
{
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

    /// <summary>
    /// Writes a UTF-8 string with a 4-byte <see cref="Int32"/> length prefix.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public void WriteString(string str)
    {
        int byteCount = Encoding.UTF8.GetByteCount(str);
        WriteInt32(byteCount);
        EnsureSpace(byteCount);

        Encoding.UTF8.GetBytes(str, _span.Slice(_pos));
        _pos += byteCount;
    }
}
