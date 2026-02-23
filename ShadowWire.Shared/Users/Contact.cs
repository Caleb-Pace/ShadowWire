namespace ShadowWire.Shared.Users;

public readonly struct Contact(string nickname, byte[] fingerprint, byte[] publicKeyDer)
{
    public readonly string Nickname { get; init; } = nickname;
    public ReadOnlyMemory<byte> Fingerprint { get; init; } = fingerprint;
    public ReadOnlyMemory<byte> PublicKeyDer { get; init; } = publicKeyDer;
}
