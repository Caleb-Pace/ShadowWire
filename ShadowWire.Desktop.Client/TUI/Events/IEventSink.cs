namespace ShadowWire.Desktop.Client.TUI.Events;

public interface IEventSink
{
    public void Post(UiEvent e);
}
