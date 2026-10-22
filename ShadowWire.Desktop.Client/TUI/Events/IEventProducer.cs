namespace ShadowWire.Desktop.Client.TUI.Events;

public interface IEventProducer
{
    public void Post(UiEvent e);
}
