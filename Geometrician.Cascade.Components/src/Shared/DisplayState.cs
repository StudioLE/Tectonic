using MudBlazor;

namespace Geometrician.Cascade.Components.Shared;

/// <summary>
/// Configuration settings for the UI.
/// </summary>
public class DisplayState
{
    /// <summary>
    /// The value of a single gap between elements.
    /// </summary>
    public int Gap { get; set; } = 4;

    /// <summary>
    /// The multiplier to use against <see cref="Gap"/> to increase the distance.
    /// </summary>
    public int GapMultiplier { get; set; } = 2;

    /// <summary>
    /// The height of mosaic element which contains tiles.
    /// </summary>
    public int MosaicHeight { get; set; } = 340;

    /// <summary>
    /// Should dark mode be enabled?
    /// </summary>
    public bool IsDarkMode { get; set; } = false;

    /// <summary>
    /// Configuration for the MudBlazor theme.
    /// </summary>
    public MudTheme Theme { get; } = new()
    {
        Palette = new()
        {
            Primary = MudBlazor.Colors.DeepOrange.Accent2,
            Secondary = MudBlazor.Colors.Indigo.Default,
            // Tertiary = MudBlazor.Colors.LightBlue.Darken3
            // Background = MudBlazor.Colors.BlueGrey.Lighten5,
            Background = MudBlazor.Colors.Shades.White,
            BackgroundGrey = MudBlazor.Colors.BlueGrey.Lighten5,
            DrawerBackground = new(MudBlazor.Colors.BlueGrey.Lighten2),
            DrawerText = new(MudBlazor.Colors.Shades.White)
        },
        PaletteDark = new PaletteDark
        {
            Primary = MudBlazor.Colors.DeepOrange.Accent2,
            Secondary = MudBlazor.Colors.Indigo.Default
            // DrawerBackground = new(MudBlazor.Colors.BlueGrey.Darken3),
            // DrawerText = new(MudBlazor.Colors.Shades.White),
        },
        LayoutProperties = new()
        {
            DefaultBorderRadius = "0px",
            AppbarHeight = "0px"
        }
    };
}
