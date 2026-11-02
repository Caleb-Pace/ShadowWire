using ShadowWire.Desktop.Client.TUI.Components.Core;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components.Primitives;

public enum BorderLine
{
    /// <summary>
    /// <code>
    /// ┌───┐
    /// │   │
    /// └───┘
    /// </code>
    /// </summary>
    Single,

    /// <summary>
    /// <code>
    /// ╭───╮
    /// │   │
    /// ╰───╯
    /// </code>
    /// </summary>
    Rounded,

    /// <summary>
    /// <code>
    /// ╔═══╗
    /// ║   ║
    /// ╚═══╝
    /// </code>
    /// </summary>
    Double,

    /// <summary>
    /// <code>
    /// ┏━━━┓
    /// ┃   ┃
    /// ┗━━━┛
    /// </code>
    /// </summary>
    Heavy,

    /// <summary>
    /// <code>
    /// +---+
    /// |   |
    /// +---+
    /// </code>
    /// </summary>
    Ascii
}

/// <summary>
/// Draws a box, optionally around a single child component. The border is drawn with
/// <see cref="BaseComponent.Style"/>; the child keeps its own style.
/// </summary>
public class Border : BaseComponent
{
    private readonly record struct Glyphs(char TL, char TR, char BL, char BR, char H, char V);
    private static Glyphs GlyphsFor(BorderLine line) => line switch
    {
        BorderLine.Single  => new('┌', '┐', '└', '┘', '─', '│'),
        BorderLine.Rounded => new('╭', '╮', '╰', '╯', '─', '│'),
        BorderLine.Double  => new('╔', '╗', '╚', '╝', '═', '║'),
        BorderLine.Heavy   => new('┏', '┓', '┗', '┛', '━', '┃'),
        BorderLine.Ascii   => new('+', '+', '+', '+', '-', '|'),
        _                  => throw new ArgumentOutOfRangeException(nameof(line))
    };

    public BaseComponent? Child { get; }

    public BorderLine Line
    {
        get => _line;
        set
        {
            if (_line == value)
                return;

            _line = value;
            Invalidate();
        }
    }
    private BorderLine _line = BorderLine.Single;

    /// <summary>
    /// Blank cells between the border and the child, on every side.
    /// </summary>
    public int Padding
    {
        get => _padding;
        set
        {
            value = Math.Max(0, value);
            if (_padding == value)
                return;

            _padding = value;
            LayoutChild();
            Invalidate();
        }
    }
    private int _padding;

    // Area inside the border and padding, relative to the border's top-left.
    private Rectangle InnerArea
    {
        get
        {
            int inset = 1 + Padding;
            return new Rectangle(
                inset, inset,
                Math.Max(0, Width - inset * 2),
                Math.Max(0, Height - inset * 2));
        }
    }

    // The child must be redrawn if the border is, as the buffer area may have been overwritten.
    public override bool IsDirty => base.IsDirty || (Child?.IsDirty ?? false);


    /// <summary>
    /// Creates a border with a fixed size. If a child is given, it is resized to fit inside.
    /// </summary>
    public Border(Point origin, Size size, BaseComponent? child = null) : base(origin, size)
    {
        Child = child;
        LayoutChild();
    }

    /// <summary>
    /// Creates a border that outlines a surface.
    /// </summary>
    /// <remarks>
    /// Matches the size of the <paramref name="surface"/>.
    /// </remarks>
    public Border(Surface surface) : base(Point.Empty, surface.Size) { }

    /// <summary>
    /// Creates a border that wraps the child at its current size.
    /// </summary>
    public Border(Point origin, BaseComponent child)
        : base(origin, new Size(child.Width + 2, child.Height + 2))
    {
        Child = child;
    }


    public override void Resize(Size newSize)
    {
        base.Resize(newSize);
        LayoutChild();
    }

    private void LayoutChild()
    {
        if (Child is null)
            return;

        var inner = InnerArea.Size;
        if (Child.Size != inner)
            Child.Resize(inner);
    }


    protected override void Draw(Surface s)
    {
        if (Width >= 2 && Height >= 2)
        {
            var g = GlyphsFor(Line);
            string horizontal = new(g.H, (Width - 2));

            // Top & Bottom
            s.Write(0, 0, (g.TL + horizontal + g.TR), Style);
            s.Write(0, (Height - 1), (g.BL + horizontal + g.BR), Style);

            // Sides
            for (int y = 1; y < (Height - 1); y++)
            {
                s.Write(0, y, g.V.ToString(), Style);
                s.Write((Width - 1), y, g.V.ToString(), Style);
            }
        }

        // Padding cells are blank, clear them in case the border moved/resized.
        if (Padding > 0)
            ClearPadding(s);


        if (Child is null)
            return;

        // If the border redrew, the child must too.
        // If only the child is dirty, this just updates it.
        if (base.IsDirty)
            Child.Invalidate();

        Child.Render(s, InnerArea.Location);
    }

    private void ClearPadding(Surface s)
    {
        var inner = InnerArea;
        int left = 1, top = 1;
        int right = Width - 1, bottom = Height - 1;

        for (int y = top; y < bottom; y++)
        {
            bool inVertical = (y >= inner.Top) && (y < inner.Bottom);
            if (inVertical)
            {
                s.Write(left, y, new string(' ', Math.Max(0, inner.Left - left)), Style);
                s.Write(inner.Right, y, new string(' ', Math.Max(0, right - inner.Right)), Style);
            }
            else
            {
                s.Write(left, y, new string(' ', Math.Max(0, right - left)), Style);
            }
        }
    }
}