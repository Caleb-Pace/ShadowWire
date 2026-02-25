using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
/// <see langword="ref struct"/>: cannot be boxed or stored on the heap.
/// </remarks>
public ref struct SpanWriter(Span<byte> span)
{
    private Span<byte> _span = span;
    private int _pos = 0; // Current write position in bytes


    /// <summary>
    /// Writes a value of the specified type from the current position.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to write (primitive or struct).</typeparam>
    /// <remarks>Advances the position by <see cref="Unsafe.SizeOf{T}"/>.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the size of <typeparamref name="T"/> exceeds the remaining capacity of the span.
    /// </exception>
    public void Write<T>(T val)
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if ((uint)size > (uint)(_span.Length - _pos))
            throw new ArgumentOutOfRangeException(nameof(T), "Attempted to write past the end of the span.");

        MemoryMarshal.Write(_span.Slice(_pos, size), in val);
        _pos += size;
    }

    /// <summary>
    /// Writes a byte array from the current position.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the length exceeds the remaining capacity of the span.
    /// </exception>
    /// <remarks>
    /// <b>No length prefix</b>.
    /// </remarks>
    public void WriteRawBytes(ReadOnlySpan<byte> bytes)
    {
        int length = bytes.Length;
        if ((uint)length > (uint)(_span.Length - _pos))
            throw new ArgumentOutOfRangeException(nameof(bytes), "Attempted to write past the end of the span.");

        // Copy bytes to the current position in the span
        bytes.CopyTo(_span.Slice(_pos));
        _pos += length;
    }

    /// <summary>
    /// Writes a byte array from the current position.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the length exceeds the remaining capacity of the span.
    /// </exception>
    /// <remarks>
    /// Byte arrays are prefixed with a 4-byte <see cref="Int32"/> length.
    /// </remarks>
    public void WriteBytes(ReadOnlySpan<byte> bytes)
    {
        Write<Int32>(bytes.Length);
        WriteRawBytes(bytes);
    }

    /// <summary>
    /// Writes a UTF-8 encoded string from the current position.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the string length exceeds the remaining capacity of the span.
    /// </exception>
    /// <remarks>
    /// Strings are prefixed with a 4-byte <see cref="Int32"/> length.
    /// </remarks>
    public void WriteString(string str)
        => WriteBytes(Encoding.UTF8.GetBytes(str));
}
