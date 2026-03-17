using ShadowWire.Shared.BinaryEncoding;
using System.Security.Cryptography;

namespace ShadowWire.Desktop.Client.Security.Algorithms.Symmetric;

public class Aes256GcmAlgorithm : ISymmetricAlgorithm
{
    public const int KEY_SIZE_IN_BITS = 256;
    public const int KEY_SIZE_IN_BYTES = KEY_SIZE_IN_BITS / sizeof(byte);

    private const int AES_BLOCK_SIZE = 128;
    public const int TAG_SIZE_IN_BYTES = AES_BLOCK_SIZE / sizeof(byte);

    private static int NounceSizeInBytes => AesGcm.NonceByteSizes.MaxSize;


    public ReadOnlyMemory<byte> GenerateSymmetricKey()
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = KEY_SIZE_IN_BITS;
            aes.GenerateKey();
            
            return aes.Key;
        }
    }


    public ReadOnlyMemory<byte> Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(key.Length, KEY_SIZE_IN_BYTES, nameof(key));

        var reader = new SpanReader(data);
        
        ReadOnlySpan<byte> nonce = reader.ReadBytes(NounceSizeInBytes);

        int ciphertextLength = (data.Length - reader.Position) - TAG_SIZE_IN_BYTES;
        ReadOnlySpan<byte> ciphertext = reader.ReadBytes(ciphertextLength);
        ReadOnlySpan<byte> tag        = reader.ReadBytes(TAG_SIZE_IN_BYTES);

        byte[] plaintext = new byte[ciphertextLength];

        using (var aes = new AesGcm(key, TAG_SIZE_IN_BYTES))
        {
            aes.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        return plaintext;
    }

    private static ReadOnlyMemory<byte> GenerateNonce()
    {
        byte[] nonceBytes = new byte[NounceSizeInBytes];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(nonceBytes);
        }

        return nonceBytes;
    }

    public ReadOnlyMemory<byte> Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(key.Length, KEY_SIZE_IN_BYTES, nameof(key));

        int length = NounceSizeInBytes + data.Length + TAG_SIZE_IN_BYTES;
        byte[] encryptedData = new byte[length];

        ReadOnlyMemory<byte> nonce = GenerateNonce();

        Span<byte> ciphertextSpan = encryptedData.AsSpan(NounceSizeInBytes, data.Length);
        Span<byte> tagSpan        = encryptedData.AsSpan(NounceSizeInBytes + data.Length);

        using (var aes = new AesGcm(key, TAG_SIZE_IN_BYTES))
        {
            aes.Encrypt(nonce.Span, data, ciphertextSpan, tagSpan);
        }

        return encryptedData;
    }
}
