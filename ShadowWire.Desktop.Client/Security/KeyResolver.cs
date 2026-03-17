using ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;

namespace ShadowWire.Desktop.Client.Security;

public class KeyResolver
{
    private readonly IAsymmetricAlgorithm _asymmetric;
    private readonly IKeyStorage _keyStorage;


    public KeyResolver(IAsymmetricAlgorithm asymmetricAlgorithm, IKeyStorage keyPairStorage)
    {
        _asymmetric = asymmetricAlgorithm;
        _keyStorage = keyPairStorage;
    }

    public DerKeyPair ResolveKeyPair()
    {
        DerKeyPair? keyPair = _keyStorage.LoadKeyPair();

        if (keyPair == null)
        {
            keyPair = _asymmetric.GenerateKeyPair();
            _keyStorage.SaveKeyPair(keyPair.Value);
        }

        return keyPair.Value;
    }
}
