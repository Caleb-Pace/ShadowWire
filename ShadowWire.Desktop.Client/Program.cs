using ShadowWire.Desktop.Client.Network;
using ShadowWire.Desktop.Client.Users;
using System.Text;

namespace ShadowWire.Desktop.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //const string URI = "ws://127.0.0.1:4960/ws/";

            //var connection = new Connection(URI);

            //await connection.SendAsync(Encoding.UTF8.GetBytes("Hello"));

            //var respBin = await connection.ReceiveAsync();
            //Console.WriteLine($"Response from the server: \"{Encoding.UTF8.GetString(respBin)}\"");

            //// TODO: Remove, for debugging
            //var crypto = new Security.Cryptography("sw-rsa-key-pub.der", "sw-rsa-key-priv.der");
            //Console.WriteLine($"Fingerprint: {Convert.ToBase64String(crypto.Fingerprint)}");

            // TODO: Remove, for debugging
            var contacts = new ContactManager("contacts.bin");

            // TODO: Remove, for debugging
            string name = "Steve";
            byte[] fingerprint = [(int)'Y', (int)'e', (int)'s'];
            var testContact = new Contact(name, fingerprint, [(int)'p', (int)'u', (int)'b']);

            // TODO: Remove, for debugging
            Console.WriteLine($"Lookup via nickname: {HasValue(contacts.Get(name))}");
            Console.WriteLine($"Lookup via fingerprint: {HasValue(contacts.Get(fingerprint))}");
            contacts.TryAdd(testContact);
            Console.WriteLine($"Lookup via nickname: {HasValue(contacts.Get(name))}");
            Console.WriteLine($"Lookup via fingerprint: {HasValue(contacts.Get(fingerprint))}");

            // TODO: Remove, for debugging
            var foundContact = contacts.Get(name);
            if (foundContact != null)
                Console.WriteLine($"Found contact: {{ Name:\"{foundContact?.Nickname}\"; FP: '{Encoding.ASCII.GetString(foundContact?.Fingerprint ?? [])}'; Pk: '{Encoding.ASCII.GetString(foundContact?.PublicKeyDer ?? [])}' }}");
        }

        private static string HasValue(Contact? contact) => (contact != null ? "Found" : "Not found");
    }
}
