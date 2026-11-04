using System.Drawing;
using System.Text;

namespace ShadowWire.Desktop.Client.TUI.Themes;

// TODO: Verify comment correctness
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