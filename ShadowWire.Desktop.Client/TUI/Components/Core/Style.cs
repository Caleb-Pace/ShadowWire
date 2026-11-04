using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Components.Core;

/// <summary>
/// A 24-bit terminal color packed into one int. <c>default</c> is the terminal's own color.
/// </summary>
/// <remarks>
/// Converts implicitly from <see cref="Color"/>. Fully transparent colors (including <see cref="Color.Empty"/>)
/// become <c>default</c>; partial alpha is ignored here, <see cref="Style"/> handles blending.
/// </remarks>
public readonly record struct TrueColor
{
    private const int SET = 1 << 24; // Distinguishes a real color (including black) from default.

    private readonly int _value;


    private TrueColor(int value)
        => _value = value;

    public static TrueColor Rgb(int rgb)
        => Color.FromArgb(unchecked((int)0xFF000000) | rgb);

    public static implicit operator TrueColor(Color c)
        => (c.A == 0) ? default : new(SET | (c.ToArgb() & 0xFFFFFF));


    /// <summary>
    /// Appends the SGR parameters (with leading ';') for this color. Nothing for default.
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
///     <item>a <see langword="default"/> <see cref="TrueColor"/> sets the terminal's default color.</item>
/// </list>
/// </remarks>
public readonly record struct Style(TrueColor? Fg = null, TrueColor? Bg = null)
{
    /// <summary>
    /// Blends a translucent <paramref name="fg"/> over an opaque-or-known <paramref name="bg"/>.
    /// </summary>
    public static Style Blend(Color fg, Color bg)
        => new(Over(fg, bg), bg);

    private static TrueColor Over(Color fg, Color bg)
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
