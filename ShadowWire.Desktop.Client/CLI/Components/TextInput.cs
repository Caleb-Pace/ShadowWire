using ShadowWire.Desktop.Client.CLI.Events;
using System.Text;

namespace ShadowWire.Desktop.Client.CLI.Components;

public class TextInput
{
    protected readonly StringBuilder _buffer = new();

    public Func<char, bool> Allowed { get; init; } = _ => true;
    public int MinLength { get; init; } = 1;
    public int MaxLength { get; init; } = int.MaxValue;
    /// <summary>
    /// When <see langword="true"/>, <c>{ Shift + Enter }</c> inserts a newline and pasted newlines are kept.
    /// </summary>
    public bool AllowNewlines { get; init; } = false;

    public int Cursor { get; private set; }
    public string Text => _buffer.ToString();
    /// <summary>
    /// <see langword="true"/> when the current text satisfies constraints.
    /// </summary>
    public bool IsValid => (_buffer.Length >= MinLength);


    /// <returns><see langword="true"/> if the event was consumed.</returns>
    public bool Handle(UiEvent e)
    {
        switch (e)
        {
            case PasteEvent p:
                Insert(p.Text);
                return true;

            case KeyEvent { KeyInfo: var k }:
                return HandleKey(k);

            default:
                return false;
        }
    }

    /// <returns><see langword="true"/> if the event was consumed.</returns>
    protected bool HandleKey(ConsoleKeyInfo k)
    {
        switch (k.Key)
        {
            // Soft newline support: { Shift + Enter }
            case ConsoleKey.Enter
            when AllowNewlines && (k.Modifiers & ConsoleModifiers.Shift) != 0:
                Insert("\n");
                return true;

            case ConsoleKey.Backspace:
                if (Cursor > 0)
                    _buffer.Remove(--Cursor, 1);
                return true;

            case ConsoleKey.Delete:
                if (Cursor < _buffer.Length)
                    _buffer.Remove(Cursor, 1);
                return true;

            case ConsoleKey.LeftArrow:
                Cursor = Math.Max(0, Cursor - 1);
                return true;

            case ConsoleKey.RightArrow:
                Cursor = Math.Min(_buffer.Length, Cursor + 1);
                return true;

            case ConsoleKey.Home:
                Cursor = 0;
                return true;

            case ConsoleKey.End:
                Cursor = _buffer.Length;
                return true;
        }

        if (k.KeyChar != '\0' && !char.IsControl(k.KeyChar))
        {
            Insert(k.KeyChar.ToString());
            return true;
        }

        return false; // Key not handled by text input, defer.
    }

    protected void Insert(string text)
    {
        // Normalise line endings
        text = text.Replace("\r\n", "\n").Replace('\r', '\n');

        foreach (var c in text)
        {
            if (c == '\n' && !AllowNewlines)
                continue; // Strip
            else if (char.IsControl(c) || !Allowed(c))
                continue; // Ignore
            if (_buffer.Length >= MaxLength)
                break;    // Limit reached.
            
            _buffer.Insert(Cursor++, c);
        }
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
        Cursor = 0;
    }
}
