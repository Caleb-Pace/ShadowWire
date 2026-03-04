using ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;

namespace ShadowWire.Desktop.Client.Security;

public class KeyResolver(IAsymmetricAlgorithm asymmetricAlgorithm, IKeyStorage keyPairStorage)
{
    private readonly IAsymmetricAlgorithm _asymmetric = asymmetricAlgorithm;
    private readonly IKeyStorage _keyStorage = keyPairStorage;


    public DerKeyPair ResolveKeyPair()
    {
        var existingKey = _keyStorage.LoadKeyPair();
        if (existingKey.HasValue)
            return existingKey.Value;

        var newKey = _asymmetric.GenerateKeyPair();
        _keyStorage.SaveKeyPair(newKey);
        return newKey;
    }
}
