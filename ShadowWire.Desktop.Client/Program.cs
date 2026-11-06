using ShadowWire.Desktop.Client.Network;
using ShadowWire.Desktop.Client.Security;
using ShadowWire.Desktop.Client.Security.Algorithms.Asymmetric;
using ShadowWire.Desktop.Client.Security.Algorithms.Hashing;
using ShadowWire.Desktop.Client.Security.Algorithms.Symmetric;
using ShadowWire.Desktop.Client.Services;
using ShadowWire.Desktop.Client.Storage;
using ShadowWire.Desktop.Client.TUI;
using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Events;
using ShadowWire.Desktop.Client.TUI.Scenes;
using ShadowWire.Desktop.Client.TUI.Themes;
using ShadowWire.Shared;
using ShadowWire.Shared.Protocol;
using ShadowWire.Shared.Protocol.Messages;
using ShadowWire.Shared.Users;
using SQLitePCL;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Version = ShadowWire.Shared.Version;

namespace ShadowWire.Desktop.Client;

internal class Program
{
    internal static Version version = new(0, 2, 0, 0);

    internal static CryptoAlgorithmSuite cryptoAlgorithms = new(
        Asymmetric: new RsaAlgorithm(HashAlgorithmName.SHA256),
        Symmetric:  new Aes256GcmAlgorithm(),
        Hashing:    new Sha256Hasher()
    );


    //private static DerKeyPair ResolveKeyPair(IKeyStorage keyStorage)
    //{

    //}

    static async Task Main(string[] args)
    {
        const string URI = "ws://127.0.0.1:4960/ws/";

        // Update encoding
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding  = Encoding.UTF8;

        
        // Initialise key resolver
        var keyStorage  = new BasicKeyStorage("sw-rsa-key-pub.der", "sw-rsa-key-priv.der");
        // TODO: Add hook for UI to show generated key
        var keyResolver = new KeyResolver(cryptoAlgorithms.Asymmetric, keyStorage);




        // Load username
        var usernameManger = new UsernameManager("sw-username.txt");
        //Console.WriteLine($"Retrieved username: \"{usernameManger.Username}\"!");

        // Resolve key pair
        var keyPair     = keyResolver.ResolveKeyPair();
        var fingerprint = Fingerprint.FromPublicKey(keyPair.PublicKey.Span);

        // Load contacts
        var contacts = new ContactManager("contacts.bin");

        // Prepare identity
        var identity = new Contact(usernameManger.Username, fingerprint, keyPair.publicKey);


        /*
        // Connect to Server
        var connection = new Connection(URI);
        var context = new ClientContext(version, identity, connection);

        // Register connection
        AuthenticationService.SendAsync(context).GetAwaiter().GetResult();

        // Wait for auth response
        var respBin = await connection.ReceiveAsync();
        var content = respBin.Length > 1 ? $"\"{Encoding.UTF8.GetString(respBin.AsSpan(1))}\"" : "";
        Console.WriteLine($"Response from the server: ({respBin[0]}:{(MessageKind)respBin[0]}) with length of {respBin.Length}; {content}");
        //*/


        //IConversationRepository conversationRepository = new SqliteConversationRepository();




        // Start TUI
        var events = new EventQueue();
        var theme = Themes.Midnight;

        var home = new Home(events, theme, contacts);

        var tui = new Tui(events, home);
        new KeyReader(events).Start(); // Start keyboard listener
        await tui.RunAsync();


        // TODO: Remove, for testing
        Console.WriteLine("\nPress any key to continue!");
        Console.ReadKey();
    }


    // TODO: Remove for testing
    public static void Run(Surface s)
    {
        TrueColor accent = TrueColor.Rgb(0x7AA2F7);
        TrueColor self   = TrueColor.Rgb(0x73DACA);
        TrueColor muted  = TrueColor.Rgb(0x8A93AD);
        TrueColor danger = TrueColor.Rgb(0xF7768E);
        TrueColor row    = TrueColor.Rgb(0x151922);
 
        int y = 1;
 
        // Colours only
        s.Write(1, y++, "Foreground only", new Style(Fg: accent));
        s.Write(1, y++, "Foreground and background", new Style(accent, row));
        s.Write(1, y++, "Terminal default colours", new Style(default(TrueColor), default(TrueColor)));
 
        // Attributes, combined with |
        s.Write(1, y++, "Bold", new Style(accent, Attrs: TextAttributes.Bold));
        s.Write(1, y++, "Dim", new Style(accent, Attrs: TextAttributes.Dim));
        s.Write(1, y++, "Italic", new Style(accent, Attrs: TextAttributes.Italic));
        s.Write(1, y++, "Bold and italic", new Style(accent, Attrs: TextAttributes.Bold | TextAttributes.Italic));
        s.Write(1, y++, "Strikethrough", new Style(muted, Attrs: TextAttributes.Strikethrough));
        s.Write(1, y++, "Invert", new Style(accent, Attrs: TextAttributes.Invert));
        s.Write(1, y++, "Blink", new Style(danger, Attrs: TextAttributes.Blink));
 
        // Underline shapes
        s.Write(1, y++, "Single", new Style(accent, Underline: UnderlineStyle.Single));
        s.Write(1, y++, "Double", new Style(accent, Underline: UnderlineStyle.Double));
        s.Write(1, y++, "Curly",  new Style(accent, Underline: UnderlineStyle.Curly));
        s.Write(1, y++, "Dotted", new Style(accent, Underline: UnderlineStyle.Dotted));
        s.Write(1, y++, "Dashed", new Style(accent, Underline: UnderlineStyle.Dashed));
 
        // Underline with its own colour
        s.Write(1, y++, "Curly red underline under blue text",
            new Style(accent, Underline: UnderlineStyle.Curly, UnderlineColour: danger));
 
        // Layering: Over fills unset parts from the style underneath
        var selectedRow = new Style(TrueColor.Rgb(0xE6EBFF), TrueColor.Rgb(0x283457));
        var unreadName  = new Style(Attrs: TextAttributes.Bold);
        var mention     = new Style(self, Underline: UnderlineStyle.Curly);
 
        s.Write(1, y++, "Bold name, row background kept", unreadName.Over(selectedRow));
        s.Write(1, y++, "Self colour, curly underline, row background kept", mention.Over(selectedRow));
 
        // Blend: translucent fg mixed into a known bg
        s.Write(1, y++, "Blended foreground", Style.Blend(Color.FromArgb(200, 0x7A, 0xA2, 0xF7), Color.FromArgb(0x15, 0x19, 0x22)));
    }
}
