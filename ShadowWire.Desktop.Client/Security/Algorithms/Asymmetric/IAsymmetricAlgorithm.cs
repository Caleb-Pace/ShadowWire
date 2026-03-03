namespace ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;

/// <summary>
/// Defines the contract for asymmetric cryptographic operations.
/// </summary>
public interface IAsymmetricAlgorithm
{
    /// <summary>
    /// Generates a new public/private key pair.
    /// </summary>
    /// <returns>A tuple containing the public and private keys.</returns>
    (ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey) GenerateKeyPair();


    /// <summary>
    /// Encrypts the specified <paramref name="data"/> using the provided <paramref name="publicKey"/>.
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="publicKey">The public key used for encryption.</param>
    /// <returns>The encrypted data.</returns>
    ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> publicKey);

    /// <summary>
    /// Decrypts the specified <paramref name="data"/> using the provided <paramref name="privateKey"/>.
    /// </summary>
    /// <param name="data">The data to decrypt.</param>
    /// <param name="privateKey">The private key used for decryption.</param>
    /// <returns>The decrypted data.</returns>
    ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> privateKey);


    /// <summary>
    /// Signs the <paramref name="data"/> using the provided <paramref name="privateKey"/>.
    /// </summary>
    /// <param name="data">The data to sign.</param>
    /// <param name="privateKey">The private key used for signing.</param>
    /// <returns>The generated signature.</returns>
    ReadOnlyMemory<byte> Sign(ReadOnlySpan<byte> data, ReadOnlySpan<byte> privateKey);

    /// <summary>
    /// Verifies the <paramref name="signature"/> using the provided <paramref name="publicKey"/>.
    /// </summary>
    /// <param name="signature">The signature to verify.</param>
    /// <param name="publicKey">The public key used for verification.</param>
    /// <returns><see langword="true"/> if the signature is valid; otherwise, <see langword="false"/>.</returns>
    bool VerifySignature(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey);
}
