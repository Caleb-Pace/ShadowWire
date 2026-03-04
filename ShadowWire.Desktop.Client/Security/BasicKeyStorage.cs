namespace ShadowWire.Desktop.Client.Security;

public class BasicKeyStorage(string publicKeyFilePath, string privateKeyFilePath) : IKeyStorage
{
    private readonly string _publicKeyFilePath = publicKeyFilePath;
    private readonly string _privateKeyFilePath = privateKeyFilePath;


    public void SaveKeyPair(DerKeyPair derKeyPair)
    {
        File.WriteAllBytes(_publicKeyFilePath, derKeyPair.PublicKey.Span);
        File.WriteAllBytes(_privateKeyFilePath, derKeyPair.PrivateKey.Span);
    }

    public DerKeyPair LoadKeyPair()
    {
        return new DerKeyPair(
            publicKey: File.ReadAllBytes(_publicKeyFilePath),
            privateKey: File.ReadAllBytes(_privateKeyFilePath)
        );
    }
}
