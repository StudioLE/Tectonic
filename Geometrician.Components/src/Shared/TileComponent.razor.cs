using Microsoft.AspNetCore.Components;
using StudioLE.Core.System;

namespace Geometrician.Components.Shared;

/// <summary>
/// A <see cref="IComponent"/> to style content as a tile.
/// Typically a series of tiles are contained in a <see cref="MosaicComponent"/>.
/// </summary>
public class TileComponentBase : TemplatedComponentBase
{
    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    private DisplayState Display { get; set; } = default!;

    /// <summary>
    /// The horizontal scale of the tile.
    /// The value should be a proportion of the container height between 0 and 1.
    /// </summary>
    [Parameter]
    public double XScale { get; set; } = 0.5;

    /// <summary>
    /// The vertical scale of the tile.
    /// The value should be a proportion of the container height.
    /// </summary>
    [Parameter]
    public double YScale { get; set; } = 0.5;

    /// <summary>
    /// Should the content be padded?
    /// </summary>
    [Parameter]
    public bool Padding { get; set; } = true;

    /// <summary>
    /// An optional title to attach to the tile.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// An optional sub title to attach to the tile.
    /// </summary>
    [Parameter]
    public string? SubTitle { get; set; }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (Padding)
            Classes.Add($"pa-{Display.GapMultiplier}");
        if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(SubTitle))
        {
            Styles["background"] = "var(--mud-palette-drawer-background)";
            Styles[" writing-mode"] = "sideways-lr";
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
