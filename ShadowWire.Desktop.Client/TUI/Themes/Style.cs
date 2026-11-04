using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Themes;

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
