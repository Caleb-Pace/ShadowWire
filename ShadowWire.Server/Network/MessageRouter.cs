using ShadowWire.Server.Handlers;
using ShadowWire.Shared.Protocol;

namespace ShadowWire.Server.Network;

internal class MessageRouter
{
    public static async Task<IEncodable?> ProcessMessageAsync(ClientSession session, byte[] buffer)
    {
        ArgumentOutOfRangeException.ThrowIfZero(buffer.Length, nameof(buffer));

        var messageKind = buffer[0];
        var messageHandler = ServerMessageHandlerRegistry.Instance.Get(messageKind);
        if (messageHandler == null)
            return null; // Unmapped/unsupported message kind

        return await messageHandler(session, buffer);
    }
}
