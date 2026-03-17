using ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;
using ShadowWire.Desktop.Client.Security.Algorithms.Hashing;
using ShadowWire.Desktop.Client.Security.Algorithms.Symmetric;

namespace ShadowWire.Desktop.Client.Security;

public readonly record struct CryptoAlgorithmSuite
(
    IAsymmetricAlgorithm Asymmetric,
    ISymmetricAlgorithm Symmetric,
    IHashingAlgorithm Hashing
);
