using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Components;


/// <summary>
/// A 24-bit terminal colour packed into one int. <c>default</c> is the terminal's own colour.
/// </summary>
/// <remarks>
/// Converts implicitly from <see cref="Color"/> (alpha is ignored; <see cref="Color.Empty"/> becomes <c>default</c>).
/// </remarks>
public readonly record struct Colour
{
    private const int Set = 1 << 24; // Distinguishes a real colour (even black) from default.

    private readonly int _value;
    private Colour(int value) => _value = value;

    public static implicit operator Colour(Color c)
        => c.IsEmpty ? default : new(Set | (c.ToArgb() & 0xFFFFFF));

    /// <summary>
    /// Appends the SGR parameters (with leading ';') for this colour. Nothing for default.
    /// </summary>
    internal void AppendSgr(StringBuilder sb, bool background)
    {
        if (_value == 0)
            return;

        sb.Append(background ? ";48;2;" : ";38;2;")
          .Append((_value >> 16) & 0xFF).Append(';')
          .Append((_value >> 8) & 0xFF).Append(';')
          .Append(_value & 0xFF);
    }
}

/// <summary>
/// Foreground and background of a cell. <c>default</c> is the terminal's own colours.
/// </summary>
public readonly record struct Style(Colour Fg, Colour Bg = default)
{
    /// <summary>
    /// Appends an SGR sequence that resets, then applies this style.
    /// </summary>
    internal void AppendSgr(StringBuilder sb)
    {
        sb.Append("\x1b[0");
        Fg.AppendSgr(sb, background: false);
        Bg.AppendSgr(sb, background: true);
        sb.Append('m');
    }
}