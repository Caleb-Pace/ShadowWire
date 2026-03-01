using ShadowWire.Shared.Users;

namespace ShadowWire.Server.Network;

public readonly struct ClientSessionConfig(Action<Guid, Fingerprint> onFingerprintChanged)
{
    public readonly Action<Guid, Fingerprint> OnFingerprintChanged { get; init; } = onFingerprintChanged;
}
