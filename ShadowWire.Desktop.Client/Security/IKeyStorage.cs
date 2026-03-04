namespace ShadowWire.Desktop.Client.Security;

public interface IKeyStorage
{
    DerKeyPair? LoadKeyPair();
    void SaveKeyPair(DerKeyPair derKeyPair);
}
