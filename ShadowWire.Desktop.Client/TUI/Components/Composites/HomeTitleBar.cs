using ShadowWire.Desktop.Client.TUI.Components.Core;
using ShadowWire.Desktop.Client.TUI.Themes;
using System.Drawing;

namespace ShadowWire.Desktop.Client.TUI.Components.Composites;

public sealed class HomeTitleBar(Theme theme, int screenWidth, Point origin = default) : BaseComponent(origin, new Size(screenWidth, 1))
{
    private Theme _theme = theme;


    public int TotalUnreadMessageCount
    {
        get;
        set
        {
            if (value == field)
                return;

            field = value;
            Invalidate();
        }
    } = 0;

    public int ContactCount
    {
        get;
        set
        {
            if (value == field)
                return;

            field = value;
            Invalidate();
        }
    } = 0;


    protected override void Draw(Surface s)
    {
        s.Fill(' ', _theme.Bar);

        // Title
        int length = 1;
        string str = "ShadowWire";
        s.Write((X + length), Y, str, _theme.BarTitle);
        length += str.Length;

        // Spacing
        str = "  ";
        s.Write((X + length), Y, str, _theme.Bar);
        length += str.Length;

        // Scene
        str = "home";
        s.Write((X + length), Y, str, _theme.Bar);
        length += str.Length;
        
        // Separator
        str = " · ";
        s.Write((X + length), Y, str, _theme.Bar);
        length += str.Length;

        // Unread count
        str = $"{TotalUnreadMessageCount} unread";
        s.Write((X + length), Y, str, _theme.Bar);

        // Contact count
        str = $"{ContactCount} contacts";
        s.Write((X + Width - str.Length - 1), Y, str, _theme.Bar);
    }
}
