using ShadowWire.Server.Handlers;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;

namespace ShadowWire.Server.Network;

internal class MessageRouter
{
    private static bool IsUserAuthenticated(ClientSession session)
        => session.ClientIdentity.HasValue;

    private static bool IsUnauthorizedRequest(ClientSession session, byte messageKind)
        => !IsUserAuthenticated(session) && (messageKind != (byte)MessageKind.AuthenticationRequest);

    public static async Task<IEncodable?> ProcessMessageAsync(ClientSession session, byte[] buffer)
    {
        ArgumentOutOfRangeException.ThrowIfZero(buffer.Length, nameof(buffer));

        var messageKind = buffer[0];
        if (IsUnauthorizedRequest(session, messageKind))
            return new Unauthenticated();

        var messageHandler = ServerMessageHandlerRegistry.Instance.Get(messageKind);
        if (messageHandler == null)
            return null; // Unmapped/unsupported message kind

        return await messageHandler(session, buffer);
    }
}
