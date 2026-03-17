using System.Security.Cryptography;

namespace ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;

public class RsaAlgorithm(HashAlgorithmName signatureHashingAlgorithm) : IAsymmetricAlgorithm
{
    private const int KEY_SIZE = 2048; // In bits

    private static RSAEncryptionPadding EncryptionPaddingMethod => RSAEncryptionPadding.OaepSHA256;
    private static RSASignaturePadding SignaturePaddingMethod => RSASignaturePadding.Pss;

    private readonly HashAlgorithmName _signatureHashingAlgorithm = signatureHashingAlgorithm;


    public DerKeyPair GenerateKeyPair()
    {
        var rsa = RSA.Create(KEY_SIZE);

        var publicKeyDer = rsa.ExportSubjectPublicKeyInfo();
        var privateKeyDer = rsa.ExportPkcs8PrivateKey();

        return new DerKeyPair(publicKeyDer, privateKeyDer);
    }


    public ReadOnlyMemory<byte> AsymmetricDecrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> privateKey)
    {
        var rsa = RSA.Create(KEY_SIZE);
        rsa.ImportRSAPrivateKey(privateKey, out int _);

        return rsa.Decrypt(data, EncryptionPaddingMethod);
    }

    public ReadOnlyMemory<byte> AsymmetricEncrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> publicKey)
    {
        var rsa = RSA.Create(KEY_SIZE);
        rsa.ImportRSAPublicKey(publicKey, out int _);

        return rsa.Encrypt(data, EncryptionPaddingMethod);
    }


    public ReadOnlyMemory<byte> Sign(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> privateKey)
    {
        var rsa = RSA.Create(KEY_SIZE);
        rsa.ImportRSAPrivateKey(privateKey, out int _);

        return rsa.SignHash(hash.ToArray(), _signatureHashingAlgorithm, SignaturePaddingMethod);
    }

    public bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey)
    {
        var rsa = RSA.Create(KEY_SIZE);
        rsa.ImportRSAPublicKey(publicKey, out int _);

        return rsa.VerifyData(hash, signature, _signatureHashingAlgorithm, SignaturePaddingMethod);
    }
}
