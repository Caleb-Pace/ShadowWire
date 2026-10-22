namespace ShadowWire.Desktop.Client.TUI.Components;
/*
public class MultilineTextInput : TextInput
{
    /// <summary>
    /// When <see langword="true"/>, <c>{ Shift + Enter }</c> inserts a newline and pasted newlines are kept.
    /// </summary>
    public bool AllowNewlines { get; init; } = false;

    //VisibleText


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

    //Reflow
}
*/