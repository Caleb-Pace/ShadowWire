namespace ShadowWire.Shared.Protocols.ErrorProtocol;

public readonly struct ErrorPacket(int errorCode, string message)
{
    public int ErrorCode { get; init; } = errorCode;
    public string Message { get; init; } = message;
}
