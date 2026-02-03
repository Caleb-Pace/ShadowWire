namespace ShadowWire.Shared.Users;

public readonly struct Contact(string nickname, byte[] fingerprint, byte[] publicKeyDer)
{
    public string Nickname { get; init; } = nickname;
    public byte[] Fingerprint { get; init; } = fingerprint;
    public byte[] PublicKeyDer { get; init; } = publicKeyDer;
}
