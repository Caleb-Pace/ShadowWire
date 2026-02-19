namespace ShadowWire.Server.Network;

public readonly struct ClientSessionConfig(Action<Guid, byte[]> onFingerprintChanged)
{
    public readonly Action<Guid, byte[]> OnFingerprintChanged { get; init; } = onFingerprintChanged;
}
