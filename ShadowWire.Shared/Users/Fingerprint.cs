using System.Runtime.InteropServices;

namespace ShadowWire.Shared.Users;

public readonly struct Fingerprint : IEquatable<Fingerprint>
{
    // Note: FINGERPRINT_LENGTH must be at least 8
    //       because GetHashCode reads fingerprint in 8-byte chunks.
    public const int LENGTH = 32; // 256-bit fingerprint

    private readonly ReadOnlyMemory<byte> _value;
    public ReadOnlySpan<byte> Span => _value.Span;


    public Fingerprint(ReadOnlyMemory<byte> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, LENGTH, nameof(value));

        _value = value;
    }

    public bool Equals(Fingerprint other)
        => _value.Span.SequenceEqual(other.Span);

    public override bool Equals(object? obj)
        => obj is Fingerprint other && Equals(other);

    public override int GetHashCode()
    {
        // Split fingerprint into 64-bit chunks
        ulong accumulator = MemoryMarshal.Read<ulong>(_value.Span.Slice(0, 8));
        for (int i = 8; i < Fingerprint.LENGTH; i += 8)
        {
            // XOR each 64-bit chunk into the accumulator
            accumulator ^= MemoryMarshal.Read<ulong>(_value.Span.Slice(i, 8));
        }

        // Fold 64-bit accumulator into 32-bit hash
        // (upper 32-bits into lower 32-bits)
        // (int cast takes the lower 32-bits)
        return (int)(accumulator ^ (accumulator >> 32));
    }

    public static bool operator == (Fingerprint left, Fingerprint right) => left.Equals(right);
    public static bool operator != (Fingerprint left, Fingerprint right) => !left.Equals(right);
}
