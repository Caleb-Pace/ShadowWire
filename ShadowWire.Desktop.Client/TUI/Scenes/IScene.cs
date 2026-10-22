using ShadowWire.Desktop.Client.TUI.Events;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public interface IScene : IEventConsumer
{
    public void Render();
}