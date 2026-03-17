namespace ShadowWire.Desktop.Client.Security;

public readonly record struct DerKeyPair(ReadOnlyMemory<byte> publicKey, ReadOnlyMemory<byte> privateKey)
{
    public ReadOnlyMemory<byte> PublicKey { get; init; } = publicKey;
    public ReadOnlyMemory<byte> PrivateKey { get; init; } = privateKey;
}
