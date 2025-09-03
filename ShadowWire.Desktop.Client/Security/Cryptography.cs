using System.Security.Cryptography;

namespace ShadowWire.Desktop.Client.Security
{
    internal class Cryptography
    {
        public byte[] Fingerprint { get { return fingerprint; } }
        private byte[] fingerprint = [];   // SHA-256 hash of public key

        private byte[] publicKeyDer = [];  // X.509 format
        private byte[] privateKeyDer = []; // PKCS#1 format

        private const string PUBLIC_KEY_FILE = "sw-rsa-key-pub.der";
        private const string PRIVATE_KEY_FILE = "sw-rsa-key-priv.der";


        public Cryptography()
        {
            ResolveKeyPair();
        }


        //=/ Key handling
        private void ResolveKeyPair()
        {
            // Generate key pair if not saved
            if (!(File.Exists(PRIVATE_KEY_FILE) && File.Exists(PUBLIC_KEY_FILE)))
            {
                // TODO: Remove, for debugging
                char pubKeyFound = File.Exists(PUBLIC_KEY_FILE) ? 'T' : 'F';
                char privKeyFound = File.Exists(PRIVATE_KEY_FILE) ? 'T' : 'F';
                Console.WriteLine($"Key pair not found, generating...    (pub? {pubKeyFound}; priv? {privKeyFound})");

                GenerateRsaKeyPair();

                Console.WriteLine($"New RSA key pair, generated!"); // TODO: Remove, for debugging
            }
            else
            {
                LoadKeyPair();
                
                Console.WriteLine($"Found RSA key pair!"); // TODO: Remove, for debugging
            }

            // Create fingerprint
            fingerprint = SHA256.HashData(publicKeyDer);
        }

        // TODO: (Later) include key encryption options
        private void GenerateRsaKeyPair()
        {
            const int KEY_SIZE = 2048; // In bits

            // Generate keys
            using (var rsa = RSA.Create(KEY_SIZE))
            {
                privateKeyDer = rsa.ExportPkcs8PrivateKey();
                publicKeyDer = rsa.ExportSubjectPublicKeyInfo();
            }

            SaveKeyPair();
        }

        private void SaveKeyPair()
        {
            File.WriteAllBytes(PUBLIC_KEY_FILE, publicKeyDer);
            File.WriteAllBytes(PRIVATE_KEY_FILE, privateKeyDer);
        }

        private void LoadKeyPair()
        {
            publicKeyDer = File.ReadAllBytes(PUBLIC_KEY_FILE);
            privateKeyDer = File.ReadAllBytes(PRIVATE_KEY_FILE);
        }
    }
}
