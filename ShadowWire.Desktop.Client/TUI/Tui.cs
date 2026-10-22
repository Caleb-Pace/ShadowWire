using ShadowWire.Desktop.Client.TUI.Events;
using ShadowWire.Desktop.Client.TUI.Scenes;
using System.Threading.Channels;

namespace ShadowWire.Desktop.Client.TUI;

public class Tui(IScene startingScene) : IEventSink
{
    private readonly Channel<UiEvent> _events = Channel.CreateUnbounded<UiEvent>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = false
    });
    private IScene _scene = startingScene;


    /// <summary>
    /// Thread-safe event posting
    /// </summary>
    public void Post(UiEvent e)
        => _events.Writer.TryWrite(e);

    public async Task RunAsync()
    {
        await foreach (UiEvent e in _events.Reader.ReadAllAsync())
        {
            if (!Dispatch(e)) return;

            // Coalesce queued events, then draw once
            while (_events.Reader.TryRead(out var nextEvent))
                if (!Dispatch(nextEvent)) return;

            _scene.Render();
        }
    }

    bool Dispatch(UiEvent e)
    {
        switch (e)
        {
            case Quit:
                return false;
            case ChangeScene c:
                _scene = c.NewScene;
                return true;
            default:
                _scene.Handle(e);
                return true;
        }
    }
}
