using System.Drawing;

namespace ShadowWire.Desktop.Client.CLI.Components;

public abstract class BaseComponent(Point origin, Size size)
{
    public Point Origin { get; init; } = origin;
    public int X => Origin.X;
    public int Y => Origin.Y;

    public Size Size { get; init; } = size;
    public int Width => Size.Width;
    public int Height => Size.Height;


    public abstract void Draw();
}
