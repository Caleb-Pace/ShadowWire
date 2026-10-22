using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components;

public abstract class BaseComponent(Point origin, Size size)
{
    public Point Origin { get; init; } = origin;
    public int X => Origin.X;
    public int Y => Origin.Y;

    public Size Size { get; private set; } = size;
    public int Width => Size.Width;
    public int Height => Size.Height;

    public bool IsDirty { get; private set; } = true; // Always draw first frame.


    /// <summary>
    /// Marks the component as needing a redraw.
    /// </summary>
    public void Invalidate()
        => IsDirty = true;

    public virtual void Resize(Size newSize)
    {
        Size = newSize;
        Invalidate();
    }

    /// <summary>
    /// Draws only if something changed.
    /// </summary>
    /// <returns><see langword="true"/> if something was drawn.</returns>
    public bool Render()
    {
        if (!IsDirty)
            return false;

        Draw();
        IsDirty = false;
        return true;
    }

    protected abstract void Draw();
}
