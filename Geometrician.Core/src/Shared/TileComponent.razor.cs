using Microsoft.AspNetCore.Components;
using StudioLE.Core.System;

namespace Geometrician.Core.Shared;

public class TileComponentBase : TemplatedComponentBase
{
    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    private DisplayState Display { get; set; } = default!;

    [Parameter]
    public double XScale { get; set; } = 0.5;

    [Parameter]
    public double YScale { get; set; } = 0.5;

    [Parameter]
    public bool Padding { get; set; } = true;

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public string? SubTitle { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Padding)
            Classes.Add($"pa-{Display.GapMultiplier}");
        if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(SubTitle))
        {
            Styles["background"] = "var(--mud-palette-drawer-background)";
            Styles[" writing-mode"] =  "sideways-lr";
        }
        SetSize();
        base.OnParametersSet();
    }

    private void SetSize()
    {
        if (XScale.ApproximatelyEquals(default) || YScale.ApproximatelyEquals(default))
            throw new("Default exception");
        int xGapCount = (1 / XScale).RoundToInt() + 1;
        int yGapCount = (1 / YScale).RoundToInt() + 1;
        double width = (Display.MosaicHeight - Display.Gap * Display.GapMultiplier * xGapCount) * XScale;
        double height = (Display.MosaicHeight - Display.Gap * Display.GapMultiplier * yGapCount) * YScale;
        Styles["width"] = width + "px";
        Styles["height"] = height + "px";
    }
}
