using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ShadowWire.Desktop.Client.Users;

/// <summary>
/// Equality comparer for fingerprints.
/// </summary>
/// <remarks>
/// A fingerprint is a hash of the public key and is used for identification.
/// </remarks>
public readonly struct FingerprintComparer : IEqualityComparer<byte[]>
{
    // Note: FINGERPRINT_LENGTH must be at least 8
    //       because GetHashCode reads fingerprint in 8-byte chunks.
    private const int FINGERPRINT_LENGTH = 32; // 256-bit fingerprint


    public bool Equals(byte[]? x, byte[]? y)
    {
        if (x == y) return true;
        if (x == null || y == null) return false;
        if (x.Length != FINGERPRINT_LENGTH || y.Length != FINGERPRINT_LENGTH) return false;

        // Use SequenceEqual for fast comparison
        return x.AsSpan().SequenceEqual(y);
    }

    public int GetHashCode([DisallowNull] byte[] obj)
    {
        if (obj.Length != FINGERPRINT_LENGTH) return 0; // Early exit: Invalid fingerprint

        // Split fingerprint into 64-bit chunks
        ulong accumulator = MemoryMarshal.Read<ulong>(obj.AsSpan(0, 8));
        for (int i = 8; i < FINGERPRINT_LENGTH; i += 8)
        {
            // XOR 64-bit chunks together
            accumulator ^= MemoryMarshal.Read<ulong>(obj.AsSpan(i, 8));
        }

        // Fold 64-bit accumulator into 32-bit hash
        // (upper 32-bits into lower 32-bits)
        return (int)(accumulator ^ (accumulator >> 32));
    }
}
