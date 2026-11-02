using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components.Core;

public abstract class BaseComponent(Point origin, Size size)
{
    public Point Origin
    {
        get => _origin;
        set
        {
            if (_origin == value) return;
            _origin = value;
            Invalidate();
        }
    }
    private Point _origin = origin;
    public int X => Origin.X;
    public int Y => Origin.Y;

    public Size Size { get; private set; } = size;
    public int Width => Size.Width;
    public int Height => Size.Height;

    /// <summary>
    /// Colours the component draws with. Defaults to the terminal's own.
    /// </summary>
    public Style Style
    {
        get => _style;
        set
        {
            if (_style == value)
                return;
            
            _style = value;
            Invalidate();
        }
    }
    private Style _style;

    public virtual bool IsDirty => _isDirty;
    private bool _isDirty = true; // Always draw first frame.


    /// <summary>
    /// Marks the component as needing a redraw.
    /// </summary>
    public void Invalidate()
        => _isDirty = true;

    public virtual void Resize(Size newSize)
    {
        Size = newSize;
        Invalidate();
    }

    /// <summary>
    /// Draws onto <paramref name="parent"/> <b>at</b> a location, only if something changed.
    /// </summary>
    /// <returns><see langword="true"/> if something was drawn.</returns>
    public bool Render(Surface parent, Point at)
    {
        if (!IsDirty)
            return false;

        Draw(parent.Slice(new Rectangle(at, Size)));
        _isDirty = false;
        return true;
    }

    /// <summary>
    /// Draws onto <paramref name="parent"/> at the component <see cref="Origin"/>, only if something changed.
    /// </summary>
    /// <returns><see langword="true"/> if something was drawn.</returns>
    public bool Render(Surface parent)
        => Render(parent, Origin);

    /// <summary>
    /// Draws the component onto the surface <paramref name="s"/>, where (0, 0) is its top-left.
    /// </summary>
    protected abstract void Draw(Surface s);
}
