using ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;
using ShadowWire.Desktop.Client.Security.Algorithms.Hashing;
using ShadowWire.Desktop.Client.Security.Algorithms.Symmetric;

namespace ShadowWire.Desktop.Client.Security;

/// <summary>
/// Facade for asymmetric, symmetric, and hashing cryptographic operations.
/// </summary>
public sealed class Cryptography(CryptoAlgorithms algorithms) : IAsymmetricAlgorithm, ISymmetricAlgorithm, IHashingAlgorithm
{
    private readonly CryptoAlgorithms _algorithms = algorithms;


    //=/ Asymmetric operations
    public ReadOnlyMemory<byte> AsymmetricDecrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> privateKey)
        => _algorithms.Asymmetric.AsymmetricDecrypt(data, privateKey);
    public ReadOnlyMemory<byte> AsymmetricEncrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> publicKey)
        => _algorithms.Asymmetric.AsymmetricEncrypt(data, publicKey);
    public DerKeyPair GenerateKeyPair()
        => _algorithms.Asymmetric.GenerateKeyPair();
    public ReadOnlyMemory<byte> Sign(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> privateKey)
        => _algorithms.Asymmetric.Sign(hash, privateKey);
    public bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey)
        => _algorithms.Asymmetric.VerifySignature(hash, signature, publicKey);


    //=/ Symmetric operations
    public ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
        => _algorithms.Symmetric.Encrypt(data, key);
    public ReadOnlyMemory<byte> GenerateSymmetricKey()
        => _algorithms.Symmetric.GenerateSymmetricKey();
    public ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
        => _algorithms.Symmetric.Decrypt(data, key);


    //=/ Hashing operations
    public ReadOnlyMemory<byte> Hash(ReadOnlySpan<byte> data)
        => _algorithms.Hashing.Hash(data);
    public ReadOnlyMemory<byte> GenerateHMAC(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
        => _algorithms.Hashing.GenerateHMAC(data, key);
}
