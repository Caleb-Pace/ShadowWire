using ShadowWire.Desktop.Client.CLI.Events;
using System.Text;

namespace ShadowWire.Desktop.Client.CLI;

public class KeyReader(IEventSink sink)
{
    private readonly IEventSink _sink = sink;
    private Thread? _thread;


    public void Start()
    {
        _thread = new Thread(Loop) { IsBackground = true, Name = "KeyReader" };
        _thread.Start();
    }

    public void Stop()
    {
        _thread?.Interrupt();
        _thread?.Join();
    }


    private void Loop()
    {
        try
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true); // Blocking

                if (Console.KeyAvailable)
                    _sink.Post(new PasteEvent(ReadBurst(key)));
                else
                    _sink.Post(new KeyEvent(key));
            }

        } catch (ThreadInterruptedException) { }
    }

    private static string ReadBurst(ConsoleKeyInfo first)
    {
        var sb = new StringBuilder();

        void Append(ConsoleKeyInfo k)
        {
            if (k.Key == ConsoleKey.Enter)
                sb.Append('\n');
            else if (!char.IsControl(k.KeyChar))
                sb.Append(k.KeyChar);
        }

        // Handle burst
        Append(first);
        while (Console.KeyAvailable)
            Append(Console.ReadKey(intercept: true));

        return sb.ToString();
    }
}
