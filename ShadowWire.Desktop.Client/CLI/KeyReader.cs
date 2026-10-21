using ShadowWire.Desktop.Client.CLI.Events;
using System.Text;

namespace ShadowWire.Desktop.Client.CLI;

public class KeyReader(IEventSink sink)
{
    protected readonly IEventSink _sink = sink;


    public void Start()
    {
        new Thread(Loop) { IsBackground = true, Name = "KeyReader" }.Start();
    }

    protected void Loop()
    {
        while (true)
        {
            var key = Console.ReadKey(intercept: true); // Blocking

            if (Console.KeyAvailable)
                _sink.Post(new PasteEvent(ReadBurst(key)));
            else
                _sink.Post(new KeyEvent(key));
        }
    }

    // Past support
    protected static string ReadBurst(ConsoleKeyInfo first)
    {
        var sb = new StringBuilder();
        Append(sb, first);
        
        while (Console.KeyAvailable)
            Append(sb, Console.ReadKey(intercept: true));

        return sb.ToString();
    }

    protected static void Append(StringBuilder sb, ConsoleKeyInfo k)
    {
        if (k.Key == ConsoleKey.Enter)
            sb.Append('\n');
        else if (!char.IsControl(k.KeyChar))
            sb.Append(k.KeyChar);
    }
}
