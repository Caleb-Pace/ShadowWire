using ShadowWire.Desktop.Client.CLI.Components;
using ShadowWire.Desktop.Client.CLI.Events;
using System.Drawing;

namespace ShadowWire.Desktop.Client.CLI.Scenes;

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
