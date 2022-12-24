using MudBlazor;

namespace Geometrician.Core.Shared;

public class DisplayState
{
    public int Elevation { get; set; }  = 0;

    public int Gap { get; set; }  = 4;

    public int GapMultiplier { get; set; } = 2;

    public int MosaicHeight { get; set; } = 340;

    public bool IsDarkMode { get; set; } = false;

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
            DrawerText = new(MudBlazor.Colors.Shades.White),
        },
        PaletteDark = new PaletteDark
        {
            Primary = MudBlazor.Colors.DeepOrange.Accent2,
            Secondary = MudBlazor.Colors.Indigo.Default,
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
