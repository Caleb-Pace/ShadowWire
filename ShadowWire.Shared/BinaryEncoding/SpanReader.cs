using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ShadowWire.Shared.BinaryEncoding;

/// <summary>
/// Reads sequential data from a <see cref="ReadOnlySpan{byte}"/>.
/// </summary>
/// <remarks>
/// Reads advance an internal position.<br/>
/// <br/>
/// <see langword="ref struct"/>: cannot be boxed or stored on the heap.
/// </remarks>
public ref struct SpanReader(ReadOnlySpan<byte> span)
{
    private ReadOnlySpan<byte> _span = span;
    private int _pos = 0; // Current read position in bytes


    /// <summary>
    /// Reads a UTF-8 encoded string from the current position.
    /// </summary>
    /// <returns>The decoded string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the string length exceeds the remaining span.
    /// </exception>
    /// <remarks>
    /// Strings are assumed to be prefixed with a 4-byte <see cref="Int32"/> length.
    /// </remarks>
    public string ReadString()
        => Encoding.UTF8.GetString(ReadBytes());

    /// <summary>
    /// Reads a byte array from the current position.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{Byte}"/> containing the bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the length exceeds the remaining span.
    /// </exception>
    /// <remarks>
    /// Byte arrays are assumed to be prefixed with a 4-byte <see cref="Int32"/> length.
    /// </remarks>
    public ReadOnlySpan<byte> ReadBytes()
    {
        int length = Read<Int32>();
        if (_pos + length > _span.Length)
            throw new ArgumentOutOfRangeException(nameof(length), "Attempted to read past the end of the span.");

        var bytes = _span.Slice(_pos, length);
        _pos += length;
        return bytes;
    }

    /// <summary>
    /// Reads a value of the specified type from the current position.
    /// </summary>
    /// <typeparam name="T">The unmanaged type to read (primitive or struct).</typeparam>
    /// <remarks>Advances the position by <see cref="Unsafe.SizeOf{T}"/>.</remarks>
    /// <returns>The value read from the span.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the size of <typeparamref name="T"/> exceeds the remaining span.
    /// </exception>
    public T Read<T>()
        where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        if (_pos + size > _span.Length)
            throw new ArgumentOutOfRangeException(nameof(T), "Attempted to read past the end of the span.");

        T val = MemoryMarshal.Read<T>(_span.Slice(_pos, size));
        _pos += size;
        return val;
    }
}
