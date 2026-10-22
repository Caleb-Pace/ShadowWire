namespace ShadowWire.Desktop.Client.TUI.Events;

public interface IEventConsumer
{
    /// <returns><see langword="true"/> if the event was consumed.</returns>
    public bool Handle(UiEvent e);
}
