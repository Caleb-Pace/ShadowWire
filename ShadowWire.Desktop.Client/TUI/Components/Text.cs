using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components;

public class Text : BaseComponent
{
    public string Content
    {
        get => _content;
        protected set
        {
            _content = NormaliseLineEndings(value);
            Resize(GetStringBounds(_content));
        }
    }
    private string _content = string.Empty;


    public Text(Point origin, string text) : base(origin, GetStringBounds(NormaliseLineEndings(text)))
        => _content = NormaliseLineEndings(text);

    public Text(Point origin, Size size) : base(origin, size) { }


    protected static string NormaliseLineEndings(string str)
        => str.Replace("\r\n", "\n").Replace('\r', '\n');

    protected static Size GetStringBounds(string str)
    {
        int width = str.Split('\n').Max(line => line.Length);
        int height = str.Split('\n').Length;

        return new Size(width, height);
    }


    protected override void Draw()
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(Content);
    }
}
