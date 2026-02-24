namespace ShadowWire.Server.Network;

public readonly struct ClientSessionConfig(Action<Guid, ReadOnlyMemory<byte>> onFingerprintChanged)
{
    public readonly Action<Guid, ReadOnlyMemory<byte>> OnFingerprintChanged { get; init; } = onFingerprintChanged;
}
