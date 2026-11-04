namespace ShadowWire.Desktop.Client.TUI.Themes;


public static class Palettes
{
    public static Palette Midnight { get; } = new()
    {
        Surface0 = TrueColor.Rgb(0x12151C),
        Surface1 = TrueColor.Rgb(0x151922),
        Surface2 = TrueColor.Rgb(0x1A1F2B),
        Surface3 = TrueColor.Rgb(0x1F2335),
        Selected = TrueColor.Rgb(0x283457),

        TextStrong = TrueColor.Rgb(0xE6EBFF),
        TextNormal = TrueColor.Rgb(0xC0CAF5),
        TextMuted  = TrueColor.Rgb(0x8A93AD),
        TextFaint  = TrueColor.Rgb(0x6B7385),

        Accent = TrueColor.Rgb(0x7AA2F7),
        Self   = TrueColor.Rgb(0x73DACA),
    };
}


public sealed record Palette
{
    public required TrueColor Surface0 { get; init; }
    public required TrueColor Surface1 { get; init; }
    public required TrueColor Surface2 { get; init; }
    public required TrueColor Surface3 { get; init; }
    public required TrueColor Selected { get; init; }

    public required TrueColor TextStrong { get; init; }
    public required TrueColor TextNormal { get; init; }
    public required TrueColor TextMuted { get; init; }
    public required TrueColor TextFaint { get; init; }

    public required TrueColor Accent { get; init; }
    public required TrueColor Self { get; init; }
}
