using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Components;

public enum Overflow
{
    Clip,
    Ellipsis
}

/// <summary>
/// A clipped, offset view onto a shared character buffer.
/// </summary>
public sealed class Surface
{
    private readonly StringBuilder _frameSB = new();

    // Cell grid indexed [row, column]. Owned by the root and shared by all slices.
    private readonly char[,] _cells;

    // Position of this surface's local (0,0) (in root buffer coordinates).
    private readonly Point _origin;

    // Writable region (in root buffer coordinates): this surface's bounds clipped by its ancestors'.
    private readonly Rectangle _clip;

    /// <summary>
    /// Size of this surface, ignoring clipping.
    /// </summary>
    public Size Size { get; }

    // Terminal cursor. Owned by the root and shared by all slices, like _cells.
    private sealed class CursorState
    {
        public Point? Position; // Null when disabled (in root buffer coordinates).
    }
    private readonly CursorState _cursor;


    /// <summary>
    /// Creates a root surface with its own buffer, filled with spaces.
    /// </summary>
    public Surface(int width, int height)
    {
        _cells  = new char[height, width];
        _cursor = new CursorState();
        _clip   = new Rectangle(0, 0, width, height);
        Size    = _clip.Size;
        Fill(' ');
    }

    // Creates a slice over an existing buffer.
    private Surface(char[,] cells, CursorState cursor, Point origin, Rectangle clip, Size size)
        => (_cells, _cursor, _origin, _clip, Size) = (cells, cursor, origin, clip, size);

    /// <summary>
    /// Creates a view of an area that shares this surface's buffer.
    /// </summary>
    /// <returns>A surface whose (0,0) is the area's top-left, clipped to this surface.</returns>
    public Surface Slice(Rectangle area)
    {
        var sliceBounds = new Rectangle(_origin.X + area.X, _origin.Y + area.Y, area.Width, area.Height);
        return new Surface(_cells, _cursor, sliceBounds.Location, Rectangle.Intersect(_clip, sliceBounds), area.Size);
    }


    public void Write(int x, int y, string text, Overflow overflow = Overflow.Clip)
    {
        int ax = _origin.X + x;
        int ay = _origin.Y + y;
        if (ay < _clip.Top || ay >= _clip.Bottom)
            return;

        int start = Math.Max(0, _clip.Left - ax);
        int end   = Math.Min(text.Length, _clip.Right - ax);
        if (end <= start)
            return;

        for (int i = start; i < end; i++)
            _cells[ay, ax + i] = text[i];

        if (overflow == Overflow.Ellipsis && end < text.Length)
            _cells[ay, ax + end - 1] = '…';
    }

    /// <summary>
    /// Fills the visible region with a character.
    /// </summary>
    public void Fill(char c)
    {
        for (int y = _clip.Top; y < _clip.Bottom; y++)
            for (int x = _clip.Left; x < _clip.Right; x++)
                _cells[y, x] = c;
    }


    /// <summary>
    /// Shows the terminal cursor at a position relative to this surface.
    /// </summary>
    /// <remarks>
    /// Ignored if the position is clipped. Stays until <see cref="HideCursor"/> is called.
    /// </remarks>
    public void SetCursor(int x, int y)
    {
        var position = new Point(_origin.X + x, _origin.Y + y);
        if (_clip.Contains(position))
            _cursor.Position = position;
    }

    public void HideCursor()
        => _cursor.Position = null;


    /// <summary>
    /// <b>Root only.</b> Writes the whole buffer to the terminal.
    /// </summary>
    public void Flush()
    {
        int height = _cells.GetLength(0);
        int width  = _cells.GetLength(1);

        _frameSB.Clear();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
                _frameSB.Append(_cells[y, x]);

            if (y < height - 1)
                _frameSB.Append('\n');
        }


        Console.CursorVisible = false; // Hide cursor to avoid flashing.
        Console.SetCursorPosition(0, 0);
        Console.Write(_frameSB);

        // Show cursor if enabled.
        if (_cursor.Position is { } p)
        {
            Console.SetCursorPosition(p.X, p.Y);
            Console.CursorVisible = true;
        }
    }
}