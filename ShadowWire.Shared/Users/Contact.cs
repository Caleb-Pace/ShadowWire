namespace ShadowWire.Shared.Users;

public readonly struct Contact(string nickname, Fingerprint fingerprint, byte[] publicKeyDer)
{
    public readonly string Nickname { get; init; } = nickname;
    public Fingerprint Fingerprint { get; init; } = fingerprint;
    public ReadOnlyMemory<byte> PublicKeyDer { get; init; } = publicKeyDer;
}
