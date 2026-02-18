using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Packets;

namespace ShadowWire.Server.Handlers;

/// <summary>
/// Handles identification-related messages from clients.
/// </summary>
/// <remarks>
/// Follows the singleton design pattern and implements <see cref="IMessageHandler"/>.
/// </remarks>
public class AuthenticationHandler : IMessageHandler<ClientSession, AuthenticationRequest>
{
    public static AuthenticationHandler Instance => new();


    public async Task<IEncodable> HandleAsync(ClientSession context, AuthenticationRequest request)
    {
        context.AssignClientIdentity(request.contact);

        return new AuthenticationSuccess();
    }
}
