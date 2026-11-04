using ShadowWire.Desktop.Client.TUI.Events;
using ShadowWire.Desktop.Client.TUI.Scenes;

namespace ShadowWire.Desktop.Client.TUI;

public class Tui(EventQueue events, IScene startingScene)
{
    private IScene _scene = startingScene;


    public async Task RunAsync()
    {
        _scene.Render(); // Initial render

        await foreach (UiEvent e in events.Reader.ReadAllAsync())
        {
            if (!Dispatch(e)) return;

            // Coalesce queued events, then draw once.
            while (events.Reader.TryRead(out var nextEvent))
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
