namespace ShadowWire.Desktop.Client.Security.Algorithms.Hashing;

/// <summary>
/// Defines the contract for cryptographic hashing and message authentication operations.
/// </summary>
public interface IHashingAlgorithm
{
    /// <summary>
    /// Computes the Hash for the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>A fixed-length digest of the <paramref name="data"/>.</returns>
    ReadOnlyMemory<byte> ComputeHash(ReadOnlySpan<byte> data);


    /// <summary>
    /// Computes a Hash-based Message Authentication Code (HMAC) for the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="key">The secret key used for the message authentication code.</param>
    /// <param name="data">The data to authenticate.</param>
    /// <returns>The computed HMAC digest.</returns>
    ReadOnlyMemory<byte> ComputeHMAC(ReadOnlySpan<byte> key, ReadOnlySpan<byte> data);
}
