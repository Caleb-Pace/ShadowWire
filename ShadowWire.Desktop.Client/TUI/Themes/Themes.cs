namespace ShadowWire.Desktop.Client.TUI.Themes;


public static class Themes
{
    public static Theme Dark { get; } = Theme.FromPalette("Dark", Palettes.Dark);
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
    public required Style MessagePreviewSelf { get; init; }
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

        Base         = S(p.TextNormal, p.Surface0),
        Bar          = S(p.TextMuted, p.Surface3),
        BarTitle     = S(p.Accent, p.Surface3, bold: true),
        Input        = S(p.TextNormal, p.Surface3),
        InputPrompt  = S(p.Accent, p.Surface3),
        Divider      = S(p.TextFaint),
        Muted        = S(p.TextMuted),
        Accent       = S(p.Accent),

        Home = new HomeTheme
        {
            Row               = S(p.TextNormal, p.Surface1),
            RowAlt            = S(p.TextNormal, p.Surface2),
            RowSelected       = S(p.TextStrong, p.Selected),
            RowSelectedMarker = S(p.Accent, p.Selected),

            ContactName        = S(p.TextMuted),
            ContactNameUnread  = S(p.TextStrong, bold: true),
            MessagePreview     = S(p.TextMuted),
            MessagePreviewSelf = S(p.Self),
            Timestamp          = S(p.TextFaint),
            UnreadBadge        = S(p.Surface0, p.Accent),
        },

        Chat = new ChatTheme
        {
            AuthorSelf  = S(p.Self, bold: true),
            AuthorOther = S(p.Accent, bold: true),
            MessageBody = S(p.TextNormal),
            Timestamp   = S(p.TextFaint),
            DayDivider  = S(p.TextFaint),
            NewMarker   = S(p.Accent),
        },
    };

    // TODO: Support richer styles
    private static Style S(TrueColor fg, TrueColor bg = default, bool bold = false, bool italic = false)
        => new(fg, bg /*, bold, italic */);
}
