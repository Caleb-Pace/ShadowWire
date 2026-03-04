namespace ShadowWire.Desktop.Client.Security.Algorithms.Hashing;

/// <summary>
/// Defines the contract for cryptographic hashing and message authentication operations.
/// </summary>
public interface IHashingAlgorithm
{
    /// <summary>
    /// Hashes the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>A fixed-length digest of the <paramref name="data"/>.</returns>
    ReadOnlyMemory<byte> Hash(ReadOnlySpan<byte> data);


    /// <summary>
    /// Generates a Hash-based Message Authentication Code (HMAC) for the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data to authenticate.</param>
    /// <param name="key">The secret key used for the message authentication code.</param>
    /// <returns>The generated HMAC digest.</returns>
    ReadOnlyMemory<byte> GenerateHMAC(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key);
}
