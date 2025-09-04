namespace ShadowWire.Desktop.Client.Users
{
    internal class Contacts
    {
        private List<Contact> contacts = new();
        private Dictionary<string, int> contactsByName = new();
        private Dictionary<byte[], int> contactsByFingerprint = new();


        //=/ Add methods
        // TODO: Implement
        //public bool TryAdd(byte[] serializedContact)
        //{
        //    return TryAdd();
        //}

        public bool TryAdd(Contact newContact)
        {
            if (contactsByFingerprint.ContainsKey(newContact.Fingerprint)
                || contactsByName.ContainsKey(newContact.Nickname))
                return false; // Early exit: duplicate detected

            // Add & index contact
            int index = contacts.Count;
            contacts.Add(newContact);
            contactsByName.Add(newContact.Nickname, index);
            contactsByFingerprint.Add(newContact.Fingerprint, index);

            // Persist contacts list
            SaveContacts();

            return true;
        }


        //=/ Get Contact methods
        public Contact? Get(byte[] fingerprint)
        {
            bool isValid = contactsByFingerprint.TryGetValue(fingerprint, out int index);
            if (!isValid) return null;

            return Get(index);
        }

        public Contact? Get(string nickname)
        {
            bool isValid = contactsByName.TryGetValue(nickname, out int index);
            if (!isValid) return null;

            return Get(index);
        }

        private Contact? Get(int index)
        {
            if (contacts.Count <= index)
            {
                // TODO: Implement logging (error)

                return null; // Invalid index
            }

            return contacts[index];
        }

        // TODO: Implement
        //=/ Contact Data Transportation methods
        //public static byte[] ExportContact(Contact contact) { }
        //public static Contact ImportContact(byte[] serializedContact) { }
    }

    public class Contact(string nickname, byte[] fingerprint, byte[] publicKeyDer)
    {
        public string Nickname { get; private set; } = nickname;
        public byte[] Fingerprint { get; private set; } = fingerprint;
        public byte[] PublicKeyDer { get; private set; } = publicKeyDer;
    }
}
