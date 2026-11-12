namespace ShadowWire.Desktop.Client.TUI.Themes;


public static class Themes
{
    public static Theme Midnight { get; } = Theme.FromPalette("Midnight", Palettes.Midnight);
}


public sealed record HomeTheme
{
    public required Style Row { get; init; }
    public required Style RowAlt { get; init; }
    public required Style RowSelected { get; init; }
    public required Style RowSelectedMarker { get; init; }

    public required Style ContactName { get; init; }
    public required Style ContactNameUnread { get; init; }
    public required Style MessagePreview { get; init; }
    public required Style MessagePreviewUnread { get; init; }
    public required Style MessagePreviewSelfPrefix { get; init; }
    public required Style Timestamp { get; init; }
    public required Style UnreadBadge { get; init; }
}

public sealed record ChatTheme
{
    public required Style AuthorSelf { get; init; }
    public required Style AuthorOther { get; init; }
    public required Style MessageBody { get; init; }
    public required Style Timestamp { get; init; }
    public required Style DayDivider { get; init; }
    public required Style NewMarker { get; init; }
}

public sealed record Theme
{
    public required string Name { get; init; }
    public required Palette Palette { get; init; }

    public required Style Base { get; init; }
    public required Style Bar { get; init; }
    public required Style BarTitle { get; init; }
    public required Style Input { get; init; }
    public required Style InputUnfocused { get; init; }
    public required Style InputPrompt { get; init; }
    public required Style Divider { get; init; }
    public required Style Muted { get; init; }
    public required Style Accent { get; init; }

    public required HomeTheme Home { get; init; }
    public required ChatTheme Chat { get; init; }


    public static Theme FromPalette(string name, Palette p) => new()
    {
        Name    = name,
        Palette = p,

        Base           = new Style(p.TextNormal, p.Surface0),
        Bar            = new Style(p.TextMuted,  p.Surface3),
        BarTitle       = new Style(p.Accent,     p.Surface3, Attrs: TextAttributes.Bold),
        Input          = new Style(p.TextNormal, p.Surface3),
        InputUnfocused = new Style(p.TextMuted,  p.Surface2),
        InputPrompt    = new Style(p.Accent,     p.Surface3),
        Divider        = new Style(p.TextFaint),
        Muted          = new Style(p.TextMuted),
        Accent         = new Style(p.Accent),

        Home = new HomeTheme
        {
            Row               = new Style(p.TextNormal, p.Surface1),
            RowAlt            = new Style(p.TextNormal, p.Surface2),
            RowSelected       = new Style(p.TextStrong, p.Selected),
            RowSelectedMarker = new Style(p.Accent,     p.Selected),

            ContactName              = new Style(p.TextMuted),
            ContactNameUnread        = new Style(p.TextStrong, Attrs: TextAttributes.Bold),
            MessagePreview           = new Style(p.TextMuted),
            MessagePreviewUnread     = new Style(p.TextNormal),
            MessagePreviewSelfPrefix = new Style(p.Self),
            Timestamp                = new Style(p.TextFaint),
            UnreadBadge              = new Style(p.Surface0, p.Accent),
        },

        Chat = new ChatTheme
        {
            AuthorSelf  = new Style(p.Self,   Attrs: TextAttributes.Bold),
            AuthorOther = new Style(p.Accent, Attrs: TextAttributes.Bold),
            MessageBody = new Style(p.TextNormal),
            Timestamp   = new Style(p.TextFaint),
            DayDivider  = new Style(p.TextFaint),
            NewMarker   = new Style(p.Accent),
        },
    };
}
