using System.Security.Cryptography;

namespace ShadowWire.Desktop.Client.Security
{
    internal class Cryptography
    {
        public byte[] Fingerprint { get { return fingerprint; } }
        private byte[] fingerprint = [];   // SHA-256 hash of public key

        public byte[] PublicKey { get { return publicKeyDer; } }
        private byte[] publicKeyDer = [];  // X.509 format
        private byte[] privateKeyDer = []; // PKCS#1 format

        private readonly string publicKeyFile;
        private readonly string privateKeyFile;


        public Cryptography(string publicKeyFile, string privateKeyFile)
        {
            this.publicKeyFile = publicKeyFile;
            this.privateKeyFile = privateKeyFile;

            ResolveKeyPair();
        }


        //=/ Key handling
        private void ResolveKeyPair()
        {
            // Generate key pair if not saved
            if (!(File.Exists(privateKeyFile) && File.Exists(publicKeyFile)))
            {
                // TODO: Remove, for debugging
                char pubKeyFound = File.Exists(publicKeyFile) ? 'T' : 'F';
                char privKeyFound = File.Exists(privateKeyFile) ? 'T' : 'F';
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
            File.WriteAllBytes(publicKeyFile, publicKeyDer);
            File.WriteAllBytes(privateKeyFile, privateKeyDer);
        }

        private void LoadKeyPair()
        {
            publicKeyDer = File.ReadAllBytes(publicKeyFile);
            privateKeyDer = File.ReadAllBytes(privateKeyFile);
        }
    }
}
