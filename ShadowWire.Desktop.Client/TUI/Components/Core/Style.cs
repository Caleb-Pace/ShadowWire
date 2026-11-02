using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Components.Core;

/// <summary>
/// A 24-bit terminal colour packed into one int. <c>default</c> is the terminal's own colour.
/// </summary>
/// <remarks>
/// Converts implicitly from <see cref="Color"/>. Fully transparent colours (including <see cref="Color.Empty"/>)
/// become <c>default</c>; partial alpha is ignored here, <see cref="Style"/> handles blending.
/// </remarks>
public readonly record struct Colour
{
    private const int Set = 1 << 24; // Distinguishes a real colour (even black) from default.

    private readonly int _value;
    private Colour(int value) => _value = value;

    public static implicit operator Colour(Color c)
        => c.A == 0 ? default : new(Set | (c.ToArgb() & 0xFFFFFF));

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
/// Foreground and background of a cell.
/// </summary>
/// <remarks>
/// <list type="bullet">
///     <item><see langword="null"/> keeps whatever is already there;</item>
///     <item>a <see langword="default"/> <see cref="Colour"/> sets the terminal's default colour.</item>
/// </list>
/// </remarks>
public readonly record struct Style(Colour? Fg = null, Colour? Bg = null)
{
    /// <summary>
    /// Blends a translucent <paramref name="fg"/> over an opaque-or-known <paramref name="bg"/>.
    /// </summary>
    public static Style Blend(Color fg, Color bg)
        => new(Over(fg, bg), bg);

    private static Colour Over(Color fg, Color bg)
    {
        if (bg.A == 0 || fg.A == 255)
            return fg;

        int a = fg.A;
        return Color.FromArgb(Mix(fg.R, bg.R), Mix(fg.G, bg.G), Mix(fg.B, bg.B));

        int Mix(int f, int b)
            => ((f * a) + (b * (255 - a)) + 127) / 255;
    }

    /// <summary>
    /// This style applied over <paramref name="under"/>: anything not set here is kept.
    /// </summary>
    public Style Over(Style under)
        => new(Fg ?? under.Fg, Bg ?? under.Bg);

    public void AppendSgr(StringBuilder sb)
    {
        sb.Append("\x1b[0");
        Fg?.AppendSgr(sb, background: false);
        Bg?.AppendSgr(sb, background: true);
        sb.Append('m');
    }
}
