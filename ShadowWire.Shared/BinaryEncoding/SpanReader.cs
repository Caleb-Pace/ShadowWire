using System.Buffers.Binary;
using System.Text;

namespace ShadowWire.Shared.BinaryEncoding;

/// <summary>
/// Reads sequential data from a <see cref="ReadOnlySpan{byte}"/>.
/// </summary>
/// <remarks>
/// Reads advance an internal position.<br/>
/// <br/>
/// Little-Endian byte ordering is used.<br/>
/// <br/>
/// <see langword="ref struct"/>: cannot be boxed or stored on the heap.
/// </remarks>
public ref struct SpanReader(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> _span = span;
    private int _pos = 0;

    /// <summary>
    /// Current read position in bytes.
    /// </summary>
    public readonly int Position => _pos;


    /// <summary>
    /// Ensures that <paramref name="newByteCount"/> bytes can be read from the current position.
    /// </summary>
    /// <param name="newByteCount">Number of bytes to read.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="newByteCount"/> exceeds the remaining span capacity.
    /// </exception>
    public readonly void EnsureSpace(int newByteCount)
    {
        if ((uint)newByteCount > (uint)(_span.Length - _pos))
            throw new ArgumentOutOfRangeException(nameof(newByteCount), "Attempted to read past the end of the span.");
    }

    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public byte ReadByte()
    {
        EnsureSpace(sizeof(byte));
        return _span[_pos++];
    }

    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public Int32 ReadInt32()
    {
        EnsureSpace(sizeof(Int32));

        var val = BinaryPrimitives.ReadInt32LittleEndian(_span.Slice(_pos));
        _pos += sizeof(Int32);
        return val;
    }

    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public UInt64 ReadUInt64()
    {
        EnsureSpace(sizeof(UInt64));

        var val = BinaryPrimitives.ReadUInt64LittleEndian(_span.Slice(_pos));
        _pos += sizeof(UInt64);
        return val;
    }

    /// <summary>
    /// Reads a byte array that was written with a 4-byte <see cref="Int32"/> length prefix.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{Byte}"/> containing the bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public ReadOnlySpan<byte> ReadBytes()
    {
        int length = ReadInt32();
        EnsureSpace(length);

        var bytes = _span.Slice(_pos, length);
        _pos += length;
        return bytes;
    }

    /// <summary>
    /// Reads a UTF-8 string that was written with a 4-byte <see cref="Int32"/> length prefix.
    /// </summary>
    /// <returns>The decoded string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If there is not enough remaining span capacity.
    /// </exception>
    public string ReadString()
        => Encoding.UTF8.GetString(ReadBytes());
}
