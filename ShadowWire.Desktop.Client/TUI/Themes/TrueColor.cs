using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Themes;

/// <summary>
/// A 24-bit terminal colour packed into one int. <c>default</c> means the terminal's own color.
/// </summary>
/// <remarks>
/// Implicit from <see cref="Color"/>. Fully transparent colors (including <see cref="Color.Empty"/>)
/// become <c>default</c>; partial alpha is dropped here, <see cref="Style.Blend"/> handles mixing.
/// </remarks>
public readonly record struct TrueColor
{
    private const int SET = 1 << 24; // Distinguishes a real color (even black) from default.

    private readonly int _value;


    private TrueColor(int value)
        => _value = value;

    public static TrueColor Rgb(int rgb)
        => Color.FromArgb(unchecked((int)0xFF000000) | rgb);

    public static implicit operator TrueColor(Color c)
        => (c.A == 0) ? default : new(SET | (c.ToArgb() & 0xFFFFFF));


    /// <summary>
    /// Appends the SGR parameters, each with a leading ';' for this color. Nothing for default.
    /// </summary>
    internal void AppendSgr(StringBuilder sb, bool background)
    {
        if (_value == 0)
            return;

        sb.Append( (background ? ";48;2;" : ";38;2;") )
          .Append((_value >> 16) & 0xFF).Append(';')
          .Append((_value >> 8) & 0xFF).Append(';')
          .Append(_value & 0xFF);
    }

    /// <summary>
    /// Appends the underline colour SGR parameters, with a leading ';'. Nothing for default.
    /// </summary>
    /// <remarks>
    /// Colon form: terminals that read only the semicolon form ignore the colour.
    /// </remarks>
    internal void AppendUnderlineSgr(StringBuilder sb)
    {
        if (_value == 0)
            return;

        sb.Append(";58:2::")
          .Append((_value >> 16) & 0xFF).Append(':')
          .Append((_value >> 8) & 0xFF).Append(':')
          .Append(_value & 0xFF);
    }
}