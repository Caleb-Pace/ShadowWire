using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;
using ShadowWire.Shared.Users;

namespace ShadowWire.Server.Handlers;

/// <summary>
/// Handles authentication-related messages from clients.
/// </summary>
/// <remarks>
/// Implements a manually initialized singleton pattern.<br/>
/// <br/>
/// <see cref="Initialize"/> <b>must be called</b> once <b>during</b> application <b>startup</b><br/>
/// before accessing <see cref="Instance"/>.
/// </remarks>
public class AuthenticationHandler : IMessageHandler<ClientSession, AuthenticationRequest>
{
    private static AuthenticationHandler? _instance;
    private readonly ContactManager _userRegistry;

    public static AuthenticationHandler Instance
        => _instance ?? throw new InvalidOperationException("Not initialized!");


    private AuthenticationHandler(ContactManager userRegistry)
        => _userRegistry = userRegistry;

    /// <exception cref="InvalidOperationException">Thrown if the handler has already been initialized.</exception>
    public static void Initialize(ContactManager userRegistry)
    {
        if (_instance != null)
            throw new InvalidOperationException("Already initialized!");

        _instance = new AuthenticationHandler(userRegistry);
    }

    public async Task<IEncodable> HandleAsync(ClientSession context, AuthenticationRequest request)
    {
        context.AssignClientIdentity(request.contact);

        _userRegistry.TryAdd(request.contact);

        return new AuthenticationSuccess();
    }
}
