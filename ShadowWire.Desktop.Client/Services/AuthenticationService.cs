using ShadowWire.Desktop.Client.Network;
using ShadowWire.Shared.Protocol.Messages;
using ShadowWire.Shared.Users;

namespace ShadowWire.Desktop.Client.Services;

public class AuthenticationService
{
    public static async Task SendAsync(Connection connection, Contact contact)
    {
        var request = new AuthenticationRequest(contact);
        var requestBinary = request.Encode();

        await connection.SendAsync(requestBinary);
    }
}
