namespace ShadowWire.Desktop.Client.Security.Algorithms.Symmetric;

/// <summary>
/// Defines the contract for symmetric cryptographic operations.
/// </summary>
public interface ISymmetricAlgorithm
{
    /// <summary>
    /// Generates a new symmetric key.
    /// </summary>
    /// <returns>A unique key.</returns>
    ReadOnlyMemory<byte> GenerateSymmetricKey();


    /// <summary>
    /// Encrypts the specified <paramref name="data"/> using the provided <paramref name="key"/>.
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="publicKey">The symmetric key used for encryption.</param>
    /// <returns>The encrypted data.</returns>
    ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key);

    /// <summary>
    /// Decrypts the specified <paramref name="data"/> using the provided <paramref name="key"/>.
    /// </summary>
    /// <param name="data">The data to decrypt.</param>
    /// <param name="privateKey">The symmetric key used to encrypt the <paramref name="data"/>.</param>
    /// <returns>The decrypted data.</returns>
    ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key);
}
