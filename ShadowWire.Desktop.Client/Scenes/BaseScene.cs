namespace ShadowWire.Desktop.Client.Scenes;

public abstract class BaseScene(Action<BaseScene> changeScene)
{
    private readonly Action<BaseScene> _changeScene = changeScene;


    public abstract void Render();

    public abstract void HandleInput(string text);
}
