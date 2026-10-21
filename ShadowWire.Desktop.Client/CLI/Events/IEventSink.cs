namespace ShadowWire.Desktop.Client.CLI.Events;

public interface IEventSink
{
    public void Post(UiEvent e);
}
