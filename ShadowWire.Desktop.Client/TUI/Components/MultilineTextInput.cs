using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components;

public class MultilineTextInput(Point origin, Size size) : TextInput(origin, size)
{
    /// <summary>
    /// When <see langword="true"/>, <c>{ Shift + Enter }</c> inserts a newline and pasted newlines are kept.
    /// </summary>
    public bool AllowNewlines { get; init; } = false;

    protected readonly List<int> _lineStarts = new() { 0 }; // Buffer index where each wrapped line begins.


    protected override bool HandleKey(ConsoleKeyInfo k)
    {
        if (base.HandleKey(k))
            return true; // Key handled

        // Soft newline support: { Shift + Enter }
        if (AllowNewlines && (k.Key == ConsoleKey.Enter) && ((k.Modifiers & ConsoleModifiers.Shift) != 0))
        {
            Insert("\n");
            return true; // Key handled
        }

         return false; // Key not handled by text input, defer.
    }

    protected override void Insert(string text)
    {
        // Normalise line endings
        text = text.Replace("\r\n", "\n").Replace('\r', '\n');

        foreach (var c in text)
        {
            if (c == '\n')
            {
                if (!AllowNewlines)
                    continue; // Strip newline
                // Keep newline
            }
            else if (char.IsControl(c) || !Allowed(c))
                continue;     // Ignore
            if (_buffer.Length >= MaxLength)
                break;        // Limit reached.
            
            _buffer.Insert(_index++, c);
        }
    }

    protected override void Reflow()
    {
        // Wrap: a new line starts after '\n' or once the line is full
        int w = Math.Max(1, Width);
        _lineStarts.Clear();
        _lineStarts.Add(0);
        for (int i = 1; i <= _buffer.Length; i++)
            if (_buffer[i - 1] == '\n' || i - _lineStarts[^1] == w)
                _lineStarts.Add(i);

        int row = _lineStarts.FindLastIndex(start => (start <= _index));

        int h = Math.Max(1, Height);
        int maxScroll = Math.Max(0, _lineStarts.Count - h);
        _hiddenAmount = Math.Clamp(_hiddenAmount, 0, maxScroll);

        if (row < _hiddenAmount)
            _hiddenAmount = row;
        else if (row - _hiddenAmount > h - 1)
            _hiddenAmount = row - (h - 1);

        // Absolute screen position.
        Cursor = new Point(X + _index - _lineStarts[row], Y + row - _hiddenAmount);
    }


    protected override void Draw()
    {
        for (int r = 0; r < Height; r++)
        {
            Console.SetCursorPosition(X, Y + r);
            Console.Write(LineText(_hiddenAmount + r).PadRight(Math.Max(1, Width)));
        }

        Console.SetCursorPosition(Cursor.X, Cursor.Y);
    }

    private string LineText(int row)
    {
        if (row >= _lineStarts.Count)
            return string.Empty;

        int start = _lineStarts[row];
        int end   = (row + 1 < _lineStarts.Count) ? _lineStarts[row + 1] : _buffer.Length;

        return _buffer.ToString(start, (end - start)).TrimEnd('\n');
    }
}
