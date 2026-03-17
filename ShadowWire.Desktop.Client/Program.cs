using ShadowWire.Desktop.Client.Network;
using ShadowWire.Desktop.Client.Security;
using ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;
using ShadowWire.Desktop.Client.Security.Algorithms.Hashing;
using ShadowWire.Desktop.Client.Security.Algorithms.Symmetric;
using ShadowWire.Desktop.Client.Services;
using ShadowWire.Shared;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Users;
using System.Security.Cryptography;
using System.Text;
using Version = ShadowWire.Shared.Version;

namespace ShadowWire.Desktop.Client;

internal class Program
{
    internal static Version version = new(0, 2, 0, 0);

    static async Task Main(string[] args)
    {
        const string URI = "ws://127.0.0.1:4960/ws/";


        // Initialise cryptography
        var cryptoAlgorithms = new CryptoAlgorithmSuite(
            Asymmetric: new RsaAlgorithm(HashAlgorithmName.SHA256),
            Symmetric: new Aes256GcmAlgorithm(),
            Hashing: new Sha256Hasher()
        );

        // Initialise key resolver
        var keyStorage = new BasicKeyStorage("sw-rsa-key-pub.der", "sw-rsa-key-priv.der");
        var keyResolver = new KeyResolver(cryptoAlgorithms.Asymmetric, keyStorage);


        // Load username
        var usernameManger = new UsernameManager("sw-username.txt");
        Console.WriteLine($"Retrieved username: \"{usernameManger.Username}\"!");

        // Resolve key pair
        var keyPair = keyResolver.ResolveKeyPair();
        var publicKeyHash = cryptoAlgorithms.Hashing.ComputeHash(keyPair.PublicKey.Span);
        var fingerprint = new Fingerprint(publicKeyHash);

        //// Load contacts
        // var contacts = new ContactManager("contacts.bin");

        // Prepare identity
        var identity = new Contact(usernameManger.Username, fingerprint, keyPair.publicKey);


        // Connect to Server
        var connection = new Connection(URI);
        var context = new ClientContext(version, identity, connection);

        // Register connection
        AuthenticationService.SendAsync(context).GetAwaiter().GetResult();

        // Wait for auth response
        var respBin = await connection.ReceiveAsync();
        var content = respBin.Length > 1 ? $"\"{Encoding.UTF8.GetString(respBin.AsSpan(1))}\"" : "";
        Console.WriteLine($"Response from the server: ({respBin[0]}:{(MessageKind)respBin[0]}) with length of {respBin.Length}; {content}");


        // TODO: Remove, for testing
        Console.WriteLine("\nPress any key to continue!");
        Console.ReadKey();
    }
}
