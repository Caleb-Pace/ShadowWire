namespace ShadowWire.Shared.Users;

public class ContactManager
{
    private readonly List<Contact> contacts = new();
    private readonly Dictionary<string, int> contactsByName = new();
    private readonly Dictionary<Fingerprint, int> contactsByFingerprint = new();

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
    private bool IsContactIndexed(Contact contact)
    {
        return contactsByFingerprint.ContainsKey(contact.Fingerprint) ||
               contactsByName.ContainsKey(contact.Nickname);
    }

    private int AddContactToList(Contact newContact)
    {
        int index = contacts.Count;
        contacts.Add(newContact);
        return index;
    }

    private void IndexContact(int index, Contact newContact)
    {
        contactsByName.Add(newContact.Nickname, index);
        contactsByFingerprint.Add(newContact.Fingerprint, index);
    }

    private bool TryAddCore(Contact newContact)
    {
        if (IsContactIndexed(newContact))
            return false; // Early exit: Duplicate detected

        int index = AddContactToList(newContact);
        IndexContact(index, newContact);

        return true;
    }

    public bool TryAdd(Contact newContact)
    {
        if (!TryAddCore(newContact))
            return false; // Early exit: Duplicate detected

        // Persist contacts list
        SaveContacts();
        return true;
    }

    public bool TryAddFromBytes(byte[] encodedContact)
    {
        if (!ContactBinaryCodec.TryDecode(encodedContact, out Contact contact))
            return false; // Early exit: Decoding failed
        return TryAdd(contact);
    }


    //=/ Get Contact methods
    private Contact? GetByIndex(int index)
    {
        // TODO: Implement logging (error)
        if (index >= contacts.Count)
            return null; // Early exit: Invalid index

        return contacts[index];
    }

    public Contact? GetByFingerprint(Fingerprint fingerprint)
    {
        bool isValid = contactsByFingerprint.TryGetValue(fingerprint, out int index);
        if (!isValid)
            return null; // Early exit: Fingerprint not indexed

        return GetByIndex(index);
    }

    public Contact? GetByNickname(string nickname)
    {
        bool isValid = contactsByName.TryGetValue(nickname, out int index);
        if (!isValid)
            return null; // Early exit: Nickname not indexed

        return GetByIndex(index);
    }
}
