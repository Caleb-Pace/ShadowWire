using ShadowWire.Desktop.Client.Network;
using ShadowWire.Shared;
using ShadowWire.Shared.Users;
using System.Text;

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
        var identityBin = ContactBinaryCodec.Encode(identity).ToArray();


        // Connect to Server
        var connection = new Connection(URI);
        await connection.SendAsync(identityBin); // Register identity

        var respBin = await connection.ReceiveAsync();
        Console.WriteLine($"Response from the server: \"{Encoding.UTF8.GetString(respBin)}\"");


        // TODO: Remove, for testing
        Console.WriteLine("\nPress any key to continue!");
        Console.ReadKey();
    }
}
