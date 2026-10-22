using ShadowWire.Desktop.Client.TUI.Events;
using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Components;

public class TextInput : BaseComponent, IEventConsumer
{
    protected readonly StringBuilder _buffer = new();

    public Func<char, bool> Allowed { get; init; } = _ => true;
    public int MinLength { get; init; } = 0;
    public int MaxLength { get; init; } = int.MaxValue;

    protected int _hiddenAmount = 0;
    protected int _index = 0; // Position in the underlying buffer.
    public Point Cursor { get; protected set; }
    
    public string Text => _buffer.ToString();
    /// <summary>
    /// The slice of text to draw. Long text is hidden on the left so the cursor stays visible.
    /// </summary>
    protected virtual string VisibleText
    {
        get
        {
            int len = Math.Min(Math.Max(1, Width), _buffer.Length - _hiddenAmount);
            return _buffer.ToString(_hiddenAmount, len);
        }
    }
    /// <summary>
    /// <see langword="true"/> when the current text satisfies constraints.
    /// </summary>
    public bool IsValid => (_buffer.Length >= MinLength);


    public TextInput(Point origin, int width) : base(origin, new Size(width, 1))
        => Cursor = origin;
    public TextInput(Point origin, Size size) : base(origin, size)
        => Cursor = origin;


    public bool Handle(UiEvent e)
    {
        bool consumed =  false;

        switch (e)
        {
            case PasteEvent p:
                Insert(p.Text);
                consumed = true;
                break;

            case KeyEvent { KeyInfo: var k }:
                consumed = HandleKey(k);
                break;

            default:
                break;
        }

        if (consumed)
        {
            Reflow();
            Invalidate(); // Redraw
        }

        return consumed;
    }

    /// <returns><see langword="true"/> if the event was consumed.</returns>
    protected virtual bool HandleKey(ConsoleKeyInfo k)
    {
        switch (k.Key)
        {
            case ConsoleKey.Backspace:
                if (_index > 0)
                    _buffer.Remove(--_index, 1);
                return true;

            case ConsoleKey.Delete:
                if (_index < _buffer.Length)
                    _buffer.Remove(_index, 1);
                return true;

            case ConsoleKey.LeftArrow:
                _index = Math.Max(0, _index - 1);
                return true;

            case ConsoleKey.RightArrow:
                _index = Math.Min(_buffer.Length, _index + 1);
                return true;

            case ConsoleKey.Home:
                _index = 0;
                return true;

            case ConsoleKey.End:
                _index = _buffer.Length;
                return true;
        }

        if (k.KeyChar != '\0' && !char.IsControl(k.KeyChar))
        {
            Insert(k.KeyChar.ToString());
            return true;
        }

        return false; // Key not handled by text input, defer.
    }

    protected virtual void Insert(string text)
    {
        foreach (var c in text)
        {
            if (char.IsControl(c) || !Allowed(c))
                continue; // Ignore
            if (_buffer.Length >= MaxLength)
                break;    // Limit reached.
            
            _buffer.Insert(_index++, c);
        }
    }

    /// <summary>
    /// Recomputes scroll offset and <see cref="Cursor"/> after any change to text, index or size.
    /// </summary>
    protected virtual void Reflow()
    {
        int w = Math.Max(1, Width);

        // Don't leave blank space on the right after deleting; keep the cursor cell reserved.
        int maxScroll = Math.Max(0, _buffer.Length - (w - 1));
        _hiddenAmount = Math.Clamp(_hiddenAmount, 0, maxScroll);

        // Scroll just enough to keep the cursor visible.
        if (_index < _hiddenAmount)
            _hiddenAmount = _index;
        else if (_index - _hiddenAmount > w - 1)
            _hiddenAmount = _index - (w - 1);

        // Absolute screen position.
        Cursor = new Point(X + _index - _hiddenAmount, Y);
    }


    public override void Resize(Size newSize)
    {
        base.Resize(newSize);
        Reflow();
    }

    protected override void Draw()
    {
        // Draw the text from the field's origin, padded so deleted characters get erased.
        Console.SetCursorPosition(X, Y);
        Console.Write(VisibleText.PadRight(Math.Max(1, Width)));

        // Put the terminal cursor where the user is typing.
        Console.SetCursorPosition(Cursor.X, Cursor.Y);
    }


    /// <summary>
    /// Use this for "submit" actions.
    /// </summary>
    /// <returns><see langword="true"/> and outputs the text (clearing the field).</returns>
    public bool TryTakeText(out string text)
    {
        if (!IsValid)
        {
            text = string.Empty;
            return false;
        }

        text = TakeText();
        return true;
    }

    /// <returns>The current text (clearing the field).</returns>
    public string TakeText()
    {
        var text = Text;
        Clear();
        return text;
    }

    public void Clear()
    {
        _buffer.Clear();
        _index = 0;
        _hiddenAmount = 0;
        
        Reflow();     // Reset Cursor
        Invalidate(); // Redraw
    }
}
