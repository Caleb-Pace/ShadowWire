using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ShadowWire.Shared.Users;

/// <summary>
/// Equality comparer for fingerprints.
/// </summary>
/// <remarks>
/// A fingerprint is a hash of the public key and is used for identification.
/// </remarks>
public readonly struct FingerprintComparer : IEqualityComparer<byte[]>, IEqualityComparer<ReadOnlyMemory<byte>>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        if (x == y) return true;
        if (x == null || y == null) return false;

        return Equals((ReadOnlyMemory<byte>)x, (ReadOnlyMemory<byte>)y);
    }

    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
        => x.Span.SequenceEqual(y.Span); // Use SequenceEqual for fast comparison

    public int GetHashCode([DisallowNull] byte[] obj)
        => GetHashCode((ReadOnlyMemory<byte>)obj);

    public int GetHashCode([DisallowNull] ReadOnlyMemory<byte> obj)
    {
        // Split fingerprint into 64-bit chunks
        ulong accumulator = BinaryPrimitives.ReadUInt64LittleEndian(obj.Span.Slice(0, 8));
        for (int i = 8; i < Fingerprint.LENGTH; i += 8)
        {
            // XOR each 64-bit chunk into the accumulator
            accumulator ^= BinaryPrimitives.ReadUInt64LittleEndian(obj.Span.Slice(i, 8));
        }

        // Fold 64-bit accumulator into 32-bit hash
        // (upper 32-bits into lower 32-bits)
        // (int cast takes the lower 32-bits)
        return (int)(accumulator ^ (accumulator >> 32));
    }
}
