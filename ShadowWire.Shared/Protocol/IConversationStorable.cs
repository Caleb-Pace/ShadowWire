using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol;

public interface IConversationStorable<TMessage> : IEncodable, IProtocolMessage
    where TMessage : struct, IConversationStorable<TMessage>
{
    static abstract MessageKind Kind { get; }
    static abstract TMessage Decode(ReadOnlySpan<byte> raw);

    Fingerprint Source { get; }
    Fingerprint Destination { get; }
    DateTimeOffset Timestamp { get; }
}
