using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Themes;


/// <summary>
/// Binary flags for text effects.
/// </summary>
/// <remarks>
/// Underlines are set through <see cref="Style.Underline"/> instead.
/// </remarks>
[Flags]
public enum TextAttributes
{
    None          = 0,
    Bold          = 1 << 0,
    Dim           = 1 << 1,
    Italic        = 1 << 2,
    Strikethrough = 1 << 3,
    Invert        = 1 << 4,
    Blink         = 1 << 5,
}

/// <summary>
/// Underline shapes.
/// </summary>
/// <remarks>
/// Anything beyond <see cref="Single"/> needs a modern terminal.
/// </remarks>
public enum UnderlineStyle
{
    None, Single, Double, Curly, Dotted, Dashed
}

/// <summary>
/// Colours, text effects and underline for a cell.
/// </summary>
/// <remarks>
/// <list type="bullet">
///     <item><see langword="null"/> colours are unset: <see cref="Over"/> fills them from the style underneath;</item>
///     <item>a <see langword="default"/> <see cref="TrueColor"/> means use the terminal's own colour;</item>
///     <item>anything still unset when written renders as the terminal default.</item>
/// </list>
/// </remarks>
public readonly record struct Style(
    TrueColor? Fg = null,
    TrueColor? Bg = null,
    TextAttributes Attrs = TextAttributes.None,
    UnderlineStyle Underline = UnderlineStyle.None,
    TrueColor? UnderlineColour = null
)
{
    private static readonly (TextAttributes Flag, string Code)[] AttrCodes =
    [
        (TextAttributes.Bold,          ";1"),
        (TextAttributes.Dim,           ";2"),
        (TextAttributes.Italic,        ";3"),
        (TextAttributes.Blink,         ";5"),
        (TextAttributes.Invert,        ";7"),
        (TextAttributes.Strikethrough, ";9"),
    ];


    /// <summary>
    /// Blends a translucent <paramref name="fg"/> into <paramref name="bg"/> by its alpha.
    /// </summary>
    public static Style Blend(Color fg, Color bg)
        => new(Over(fg, bg), bg);

    /// <returns><paramref name="fg"/> untouched when it is opaque or there is no <paramref name="bg"/> to mix with.</returns>
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
    /// <remarks>
    /// Unset colours and an unset underline fall through to <paramref name="under"/>.<br/>
    /// Attributes combine, so a top style can't switch one off.
    /// </remarks>
    public Style Over(Style under)
        => new(Fg ?? under.Fg,
               Bg ?? under.Bg,
               Attrs | under.Attrs,
               (Underline != UnderlineStyle.None) ? Underline : under.Underline,
               UnderlineColour ?? under.UnderlineColour);

    /// <summary>
    /// Appends the full SGR sequence for this style, starting with a reset.
    /// </summary>
    /// <remarks>
    /// Underline shapes and colour use the colon form (<c>4:3</c>, <c>58:2::r:g:b</c>).<br/>
    /// Terminals without support show a plain underline, or none.
    /// </remarks>
    public void AppendSgr(StringBuilder sb)
    {
        sb.Append("\x1b[0");

        foreach (var (flag, code) in AttrCodes)
            if (Attrs.HasFlag(flag))
                sb.Append(code);

        if (Underline != UnderlineStyle.None)
        {
            sb.Append(Underline switch
            {
                UnderlineStyle.Double => ";4:2",
                UnderlineStyle.Curly  => ";4:3",
                UnderlineStyle.Dotted => ";4:4",
                UnderlineStyle.Dashed => ";4:5",
                _                     => ";4", // Single underline (most widely supported).
            });

            UnderlineColour?.AppendUnderlineSgr(sb);
        }

        Fg?.AppendSgr(sb, background: false);
        Bg?.AppendSgr(sb, background: true);

        sb.Append('m');
    }
}
