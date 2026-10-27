using System.Buffers.Binary;
using System.Security.Cryptography;

namespace ShadowWire.Shared.Users;

public readonly struct Fingerprint : IEquatable<Fingerprint>
{
    // Note: must be at least 8
    //       because GetHashCode reads fingerprint in 8-byte chunks.
    public const int SIZE = 32; // 256-bit fingerprint

    private readonly int _hashCode;

    public ReadOnlyMemory<byte> Memory => _value;
    public ReadOnlySpan<byte> Span => _value.Span;
    private readonly ReadOnlyMemory<byte> _value;


    public Fingerprint(ReadOnlyMemory<byte> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, SIZE, nameof(value));

        _value = value;
        _hashCode = ComputeHashCode();
    }

    /// <summary>
    /// Generates a Fingerprint from a given  public key.
    /// </summary>
    /// <param name="publicKeyDer">The public key as DER-encoded SubjectPublicKeyInfo (SPKI).</param>
    /// <exception cref="ArgumentException">Thrown if the key is empty.</exception>
    public static Fingerprint FromPublicKey(ReadOnlySpan<byte> publicKeyDer)
    {
        if (publicKeyDer.IsEmpty)
            throw new ArgumentException("Public key must not be empty.", nameof(publicKeyDer));

        return new Fingerprint( SHA256.HashData(publicKeyDer) );
    }


    private int ComputeHashCode()
    {
        // Split fingerprint into 64-bit chunks
        ulong accumulator = BinaryPrimitives.ReadUInt64LittleEndian(_value.Span.Slice(0, 8));
        for (int i = 8; i < Fingerprint.SIZE; i += 8)
        {
            // XOR each 64-bit chunk into the accumulator
            accumulator ^= BinaryPrimitives.ReadUInt64LittleEndian(_value.Span.Slice(i, 8));
        }

        // Fold 64-bit accumulator into 32-bit hash
        // (upper 32-bits into lower 32-bits)
        // (int cast takes the lower 32-bits)
        return (int)(accumulator ^ (accumulator >> 32));
    }

    public override int GetHashCode()
        => _hashCode;

    public bool Equals(Fingerprint other)
        => _value.Span.SequenceEqual(other.Span);

    public override bool Equals(object? obj)
        => obj is Fingerprint other && Equals(other);


    public static bool operator ==(Fingerprint left, Fingerprint right)
        => left.Equals(right);
    public static bool operator !=(Fingerprint left, Fingerprint right)
        => !left.Equals(right);
}
