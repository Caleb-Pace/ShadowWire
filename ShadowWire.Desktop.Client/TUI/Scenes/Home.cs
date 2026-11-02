using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Components.Primitives;
using ShadowWire.Desktop.Client.TUI.Events;
using ShadowWire.Shared.Users;
using System.Collections;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public sealed class Home : IScene
{
    private Surface _rootSurface;
    private BaseComponent[] _componentsRef;

    private VerticalList<BaseComponent> _list; // TODO: Select Contact card composite component.
    private TextInput _input;
    private Border _border;


    public Home(ContactManager contacts)
    {
        int screenHeight = Console.WindowHeight;
        int screenWidth  = Console.WindowWidth;

        _rootSurface = new Surface(screenWidth, screenHeight);

        // Flush style
        _rootSurface.Fill(' ', new Style(Color.White, Color.Gray));
        _rootSurface.Flush();


        // Components
        _list = new VerticalList<BaseComponent>(new Point(1, 1), new Size(screenWidth - 2, screenHeight - 3))
        {
            Style = new Style(Bg: Color.Green)
        };

        _input = new TextInput(new Point(1, (screenHeight - 2)), screenWidth - 2)
        {
            Style = new Style(Color.Red, default(Color)),
            MaxLength = screenWidth - 2,
        };

        _border = new Border(_rootSurface)
        {
            Line = BorderLine.Rounded
        };


        _componentsRef = [ _list, _input, _border ];
    }


    public bool Handle(UiEvent e)
    {
        bool consumed = false;

        if (_input.IsFocused)
            consumed |= _input.Handle(e);

        consumed |= _list.Handle(e);


        if (consumed)
            return true;

        if ((e is KeyEvent k) && (k.KeyInfo.Key == ConsoleKey.Enter))
        {
            if (!_input.TryTakeText(out string text))
                return false;

            _input.IsFocused = false;
            _rootSurface.HideCursor();

            _rootSurface.Write(1, 0, $"\"{text.Replace("\n", "\\n")}\"");
        }

        return false;
    }

    public void Render()
    {
        // Must be done before any render calls
        //     (Render calls change dirty state)
        bool anyDirty = _componentsRef.Any(comp => comp.IsDirty);


        // Redraw
        _list.Render(_rootSurface);
        _input.Render(_rootSurface);
        _border.Render(_rootSurface);


        // Flush to Console
        if (anyDirty)
            _rootSurface.Flush();
    }
}
