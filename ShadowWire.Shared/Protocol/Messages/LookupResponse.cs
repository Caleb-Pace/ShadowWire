using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public sealed class LookupResponse : ContactMessageBase
{
    protected override MessageKind Kind => MessageKind.LookupResponse;


    public LookupResponse(Contact contact) : base(contact) { }
    public LookupResponse(ReadOnlySpan<byte> messageBytes) : base(messageBytes) { }
}