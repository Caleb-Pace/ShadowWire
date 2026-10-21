using ShadowWire.Desktop.Client.CLI.Events;

namespace ShadowWire.Desktop.Client.CLI.Scenes;

public interface IScene
{
    public void Handle(UiEvent e);
    public void Render();
}