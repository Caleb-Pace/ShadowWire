using ShadowWire.Desktop.Client.Network;
using ShadowWire.Desktop.Client.Services;
using ShadowWire.Shared;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Users;

namespace ShadowWire.Desktop.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        const string URI = "ws://127.0.0.1:4960/ws/";


        // Load username
        var usernameManger = new UsernameManager("sw-username.txt");
        Console.WriteLine($"Retrieved username: \"{usernameManger.Username}\"!");

        // Resolve key pair
        var crypto = new Security.Cryptography("sw-rsa-key-pub.der", "sw-rsa-key-priv.der");

        // Load contacts
        var contacts = new ContactManager("contacts.bin");
        
        // Prepare identity
        var identity = new Contact(usernameManger.Username, crypto.Fingerprint, crypto.PublicKey);
        var identityBin = ContactBinaryCodec.Encode(identity);


        // Connect to Server
        var connection = new Connection(URI);

        // Register connection
        AuthenticationService.SendAsync(connection, identity).GetAwaiter().GetResult();

        // Wait for auth response
        var respBin = await connection.ReceiveAsync();
        Console.WriteLine($"Response from the server: '{respBin[0]}' ({(MessageKind)respBin[0]}) with length of {respBin.Length}");


        // TODO: Remove, for testing
        Console.WriteLine("\nPress any key to continue!");
        Console.ReadKey();
    }
}
