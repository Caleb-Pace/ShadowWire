using ShadowWire.Shared.Users;

namespace ShadowWire.Shared.Protocol.Messages;

public sealed class AuthenticationRequest : ContactMessageBase
{
    protected override MessageKind Kind => MessageKind.LookupResponse;


    public AuthenticationRequest(Contact contact) : base(contact) { }
    public AuthenticationRequest(ReadOnlySpan<byte> messageBytes) : base(messageBytes) { }
}