using ShadowWire.Desktop.Client.Network;
using ShadowWire.Shared;
using ShadowWire.Shared.Users;
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

            //// TODO: Remove, for debugging
            //var contacts = new ContactManager("contacts.bin");

            //// TODO: Remove, for debugging
            //string name = "Steve";
            //byte[] fingerprint = [(int)'E', (int)'l', (int)'e', (int)'p', (int)'h', (int)'a', (int)'n', (int)'t'];
            //var testContact = new Contact(name, fingerprint, [(int)'p', (int)'u', (int)'b']);


            // TODO: Remove, for testing
            var usernameManger = new UsernameManager("username.txt");
            Console.WriteLine($"Retrieved username: \"{usernameManger.Username}\"!");
        }
    }
}
