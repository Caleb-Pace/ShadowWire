using ShadowWire.Shared.Protocol.Messages;

namespace ShadowWire.Desktop.Client.Services;

public class AuthenticationService
{
    public static async Task SendAsync(ClientContext context)
    {
        var request = new AuthenticationRequest(context.Version, context.ClientIdentity);
        var requestBinary = request.Encode();

        await context.Connection.SendAsync(requestBinary);
    }
}
