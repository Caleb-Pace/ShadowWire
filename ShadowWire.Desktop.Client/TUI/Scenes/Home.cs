using ShadowWire.Desktop.Client.TUI.Components;
using ShadowWire.Desktop.Client.TUI.Events;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public class Home : IScene
{
    protected TextInput testComp = new TextInput(new Point(0,0), 20);


    public void Handle(UiEvent e)
    {
        testComp.Handle(e);
    }

    public void Render()
    {
        testComp.Draw();
    }
}
