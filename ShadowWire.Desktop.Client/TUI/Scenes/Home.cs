using ShadowWire.Desktop.Client.TUI.Components;
using ShadowWire.Desktop.Client.TUI.Events;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Scenes;

public class Home : IScene
{
    private Surface _rootSurface;

    protected TextInput _input = new MultilineTextInput(new Point(0, 1), new(5, 3))
    {
        AllowNewlines = true,
    };
    private bool _inputEnabled = false;

    private VerticalList<Text> _list = new(new Point(1, 1), new Size(5, 3));


    public Home()
    {
        _rootSurface = new Surface(50, 25);

        // TODO: Remove, for debugging
        _rootSurface.Fill('#', new Style(default, Color.Pink) );
        _rootSurface.Flush();

        var overflowType = Overflow.Ellipsis;
        _list.Add( new Text(Point.Empty, "abcdef") { Style = new Style(Color.Beige, Color.FromArgb(0, 191, 255)), OverflowType = overflowType } );
        _list.Add( new Text(Point.Empty, "0")      { Style = new Style(Color.FromArgb(0, 255, 255, 255), Color.Gray), OverflowType = overflowType } );
        _list.Add( new Text(Point.Empty, " 1")     { Style = new Style(Color.FromArgb(51, 255, 255, 255), Color.Black), OverflowType = overflowType } );
        _list.Add( new Text(Point.Empty, "  2")    { Style = new Style(Color.FromArgb(102, 255, 255, 255), Color.Gray), OverflowType = overflowType } );
        _list.Add( new Text(Point.Empty, "   3")   { Style = new Style(Color.FromArgb(153, 255, 255, 255), Color.Black), OverflowType = overflowType } );
        _list.Add( new Text(Point.Empty, "    4")  { Style = new Style(Color.FromArgb(204, 255, 255, 255), Color.Gray), OverflowType = overflowType } );
        _list.Add( new Text(Point.Empty, "     5") { Style = new Style(Color.FromArgb(255, 255, 255, 255), Color.Black), OverflowType = overflowType } );
    }


    public bool Handle(UiEvent e)
    {
        bool consumed = false;

        if (_inputEnabled)
            consumed |= _input.Handle(e);

        consumed |= _list.Handle(e);


        if (consumed)
            return true;

        if ((e is KeyEvent k) && (k.KeyInfo.Key == ConsoleKey.Enter))
        {
            if (!_input.TryTakeText(out string text))
                return false;

            _inputEnabled = false;
            _input.IsFocused = false;

            Console.SetCursorPosition(_input.X, (_input.Y + _input.Height + 1));
            Console.WriteLine($"\"{text.Replace("\n", "\\n")}\"");
        }

        return false;
    }

    public void Render()
    {
        // Must be done before any render calls
        //     (Render calls change dirty state)
        bool anyDirty = _input.IsDirty || _list.IsDirty;


        // Redraw
        if (anyDirty)
            _rootSurface.Write(0, 0, "01234567890123456789", style: new Style(Color.Beige)); // TODO: Remove, for debugging
        
        //_input.Render();
        _list.Render(_rootSurface);


        // Flush to Console
        if (anyDirty)
            _rootSurface.Flush();
    }
}
