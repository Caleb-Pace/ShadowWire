using System.Security.Cryptography;

namespace ShadowWire.Desktop.Client.Security.Algorithms.Hashing;

public sealed class Sha256Hasher : IHashingAlgorithm
{
    public ReadOnlyMemory<byte> ComputeHash(ReadOnlySpan<byte> data)
        => SHA256.HashData(data);

    public ReadOnlyMemory<byte> ComputeHMAC(ReadOnlySpan<byte> key, ReadOnlySpan<byte> data)
        => HMACSHA256.HashData(key, data);
}
