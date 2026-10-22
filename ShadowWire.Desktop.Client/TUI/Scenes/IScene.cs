using ShadowWire.Desktop.Client.TUI.Events;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public interface IScene
{
    public void Handle(UiEvent e);
    public void Render();
}