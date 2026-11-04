using System.Threading.Channels;

namespace ShadowWire.Desktop.Client.TUI.Events;

public sealed class EventQueue : IEventProducer
{
    private readonly Channel<UiEvent> _events = Channel.CreateUnbounded<UiEvent>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    public ChannelReader<UiEvent> Reader => _events.Reader;

    /// <summary>
    /// Thread-safe event posting
    /// </summary>
    public void Post(UiEvent e)
        => _events.Writer.TryWrite(e);
}
