using ShadowWire.Server.Network;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Users;

namespace ShadowWire.Server.Handlers;

/// <summary>
/// Handles identification-related messages from clients.
/// </summary>
/// <remarks>
/// Follows the singleton design pattern and implements <see cref="IMessageHandler"/>.
/// </remarks>
public class AuthenticationHandler : IMessageHandler
{
    public static AuthenticationHandler Instance => new();


    private void AttachClientIdentityToSession(ClientSession session, ReadOnlySpan<byte> contactBinary)
    {
        if (ContactBinaryCodec.TryDecode(contactBinary, out var contact))
            session.AssignClientIdentity(contact);
    }

    /// <summary>
    /// Handles incoming identification messages.
    /// </summary>
    /// <param name="session">The client session.</param>
    /// <param name="messageInBytes">The message bytes, including the kind byte.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="session"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="messageInBytes"/> is too short to be a valid identification message.
    /// </exception>
    public async Task HandleAsync(ClientSession session, byte[] messageInBytes)
    {
        // Validation
        ArgumentNullException.ThrowIfNull(session);
        ArgumentOutOfRangeException.ThrowIfLessThan(messageInBytes.Length, 2, nameof(messageInBytes));

        // Handle
        byte kind = messageInBytes[0];
        ReadOnlySpan<byte> content = messageInBytes.AsSpan(1); // Skip message kind
        switch (kind)
        {
            case (byte)MessageKind.AuthenticationRequest:
                AttachClientIdentityToSession(session, content); 
                break;
            default:
                // TODO: Implement logging - Warning: Unsupported message kind
                break;
        }
    }
}
