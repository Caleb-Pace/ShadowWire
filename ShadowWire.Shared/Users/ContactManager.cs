namespace ShadowWire.Shared.Users;

public class ContactManager
{
    private List<Contact> contacts = new();
    private Dictionary<string, int> contactsByName = new();
    private Dictionary<byte[], int> contactsByFingerprint = new(new FingerprintComparer());

    private readonly string contactsFile;


    public ContactManager(string file)
    {
        contactsFile = file;
        LoadContacts();
        Console.WriteLine($"{contacts.Count} contacts loaded! (lN: {contactsByName.Count}| lF: {contactsByFingerprint.Count})"); // TODO: Remove, for debugging
    }


    //=/ Persistence
    private void LoadContacts()
    {
        if (!File.Exists(contactsFile)) return; // Early exit: No file to read

        // Clear old data
        contacts.Clear();
        contactsByName.Clear();
        contactsByFingerprint.Clear();

        using (var fileStream = File.OpenRead(contactsFile))
        using (var binReader = new BinaryReader(fileStream))
        {
            while (fileStream.Position < fileStream.Length)
            {
                int length = binReader.ReadInt32();
                byte[] data = binReader.ReadBytes(length);

                if (!ContactBinaryCodec.TryDecode(data, out Contact contact)) continue;

                // Add & index contact
                TryAddCore(contact);
            }
        }
    }

    private void SaveContacts()
    {
        using (var fileStream = File.Open(contactsFile, FileMode.Create))
        using (var binWriter = new BinaryWriter(fileStream))
        {
            foreach (var contact in contacts)
            {
                var data = ContactBinaryCodec.Encode(contact);
                binWriter.Write(data.Length);
                binWriter.Write(data);
            }
        }
    }

    //=/ Add methods
    // TODO: Implement
    //public bool TryAdd(byte[] serializedContact)
    //{
    //    return TryAdd();
    //}

    public bool TryAdd(Contact newContact)
    {
        if (!TryAddCore(newContact)) return false;

        // Persist contacts list
        SaveContacts();
        return true;
    }

    private bool TryAddCore(Contact newContact)
    {
        // Duplication check
        if (contactsByFingerprint.ContainsKey(newContact.Fingerprint) ||
            contactsByName.ContainsKey(newContact.Nickname))
            return false; // Early exit: duplicate detected

        // Add contact
        int index = contacts.Count;
        contacts.Add(newContact);

        // Index contact
        contactsByName.Add(newContact.Nickname, index);
        contactsByFingerprint.Add(newContact.Fingerprint, index);

        return true;
    }


    //=/ Get Contact methods
    public Contact? GetByFingerprint(byte[] fingerprint)
    {
        bool isValid = contactsByFingerprint.TryGetValue(fingerprint, out int index);
        if (!isValid) return null;

        return GetByIndex(index);
    }

    public Contact? GetByNickname(string nickname)
    {
        bool isValid = contactsByName.TryGetValue(nickname, out int index);
        if (!isValid) return null;

        return GetByIndex(index);
    }

    private Contact? GetByIndex(int index)
    {
        if (index >= contacts.Count)
        {
            // TODO: Implement logging (error)
            return null; // Invalid index
        }

        return contacts[index];
    }
}
