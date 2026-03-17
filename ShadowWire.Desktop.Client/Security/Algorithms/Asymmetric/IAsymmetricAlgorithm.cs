namespace ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;

/// <summary>
/// Defines the contract for asymmetric cryptographic operations.
/// </summary>
public interface IAsymmetricAlgorithm
{
    /// <summary>
    /// Generates a new public/private key pair in the DER format.
    /// </summary>
    /// <returns>A tuple containing the public and private keys.</returns>
    DerKeyPair GenerateKeyPair();


    /// <summary>
    /// Encrypts the specified <paramref name="data"/> using the provided <paramref name="publicKey"/>.
    /// </summary>
    /// <param name="data">The data to encrypt.</param>
    /// <param name="publicKey">The public key used for encryption.</param>
    /// <returns>The encrypted data.</returns>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Thrown if the key is invalid or data is too large.</exception>
    ReadOnlyMemory<byte> AsymmetricEncrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> publicKey);

    /// <summary>
    /// Decrypts the specified <paramref name="data"/> using the provided <paramref name="privateKey"/>.
    /// </summary>
    /// <param name="data">The data to decrypt.</param>
    /// <param name="privateKey">The private key used for decryption.</param>
    /// <returns>The decrypted data.</returns>
    ReadOnlyMemory<byte> AsymmetricDecrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> privateKey);


    /// <summary>
    /// Signs the <paramref name="hash"/> using the provided <paramref name="privateKey"/>.
    /// </summary>
    /// <param name="hash">The hashed data to sign.</param>
    /// <param name="privateKey">The private key used for signing.</param>
    /// <returns>The generated digital signature.</returns>
    /// <exception cref="System.Security.Cryptography.CryptographicException">Thrown if the key is invalid or data is too large.</exception>
    ReadOnlyMemory<byte> Sign(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> privateKey);

    /// <summary>
    /// Verifies the digital <paramref name="signature"/> using the provided <paramref name="publicKey"/>.
    /// </summary>
    /// <param name="hash">The hashed data to verify.</param>
    /// <param name="signature">The digital signature to verify.</param>
    /// <param name="publicKey">The public key used for verification.</param>
    /// <returns><see langword="true"/> if the signature is valid; otherwise, <see langword="false"/>.</returns>
    bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey);
}
