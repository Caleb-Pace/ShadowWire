namespace ShadowWire.Shared.Users;

public static class ContactBinaryCodec
{
    public static byte[] Serialize(Contact contact)
    {
        var memStream = new MemoryStream();

        using (var binWrite = new BinaryWriter(memStream))
        {
            binWrite.Write(contact.Nickname);

            binWrite.Write(contact.Fingerprint.Length);
            binWrite.Write(contact.Fingerprint);

            binWrite.Write(contact.PublicKeyDer.Length);
            binWrite.Write(contact.PublicKeyDer);
        }

        return memStream.ToArray();
    }

    // TODO: Implement error handling
    public static Contact? Deserialize(byte[] serializedContact)
    {
        Contact? contact = null;

        using (var memStream = new MemoryStream(serializedContact))
        using (var binReader = new BinaryReader(memStream))
        {
            int length;

            string nickname = binReader.ReadString();

            length = binReader.ReadInt32();
            byte[] fingerprint = binReader.ReadBytes(length);

            length = binReader.ReadInt32();
            byte[] publicKeyDer = binReader.ReadBytes(length);

            contact = new Contact(nickname, fingerprint, publicKeyDer);
        }

        return contact;
    }
}
