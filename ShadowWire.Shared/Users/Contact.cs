namespace ShadowWire.Shared.Users;

public class Contact(string nickname, byte[] fingerprint, byte[] publicKeyDer)
{
    public string Nickname { get; private set; } = nickname;
    public byte[] Fingerprint { get; private set; } = fingerprint;
    public byte[] PublicKeyDer { get; private set; } = publicKeyDer;
}
