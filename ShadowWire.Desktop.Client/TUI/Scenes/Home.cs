using ShadowWire.Desktop.Client.TUI.Components;
using ShadowWire.Desktop.Client.TUI.Events;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public class Home : IScene
{
    protected MultilineTextInput _input = new MultilineTextInput(new Point(0, 0), new(5, 3))
    {
        AllowNewlines = true,
    };
    private bool _inputEnabled = true;


    public bool Handle(UiEvent e)
    {
        bool consumed = false;

        if (_inputEnabled)
            consumed |= _input.Handle(e);


        if (consumed)
            return true;

        if ((e is KeyEvent k) && (k.KeyInfo.Key == ConsoleKey.Enter))
        {
            if (!_input.TryTakeText(out string text))
                return false;

            _inputEnabled = false;
            Console.CursorVisible = false;

            Console.SetCursorPosition(_input.X, (_input.Y + _input.Height + 1));
            Console.WriteLine($"\"{text.Replace("\n", "\\n")}\"");
        }

        return false;
    }

    public void Render()
    {
        bool anyDirty = _input.IsDirty;

        if (anyDirty)
            Console.CursorVisible = false; // Hide cursor during draws to avoid flashing.

        // Redraw
        _input.Render();

        if (anyDirty && _inputEnabled)
            Console.CursorVisible = true;
    }
}
