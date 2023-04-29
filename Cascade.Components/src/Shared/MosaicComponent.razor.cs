using Microsoft.AspNetCore.Components;

namespace Cascade.Components.Shared;

/// <summary>
/// A <see cref="IComponent"/> container for <see cref="TileComponent"/>.
/// </summary>
public class MosaicComponentBase : TemplatedComponentBase
{
    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <summary>
    /// Should the child content have padding?
    /// </summary>
    [Parameter]
    public bool Padding { get; set; } = true;

    /// <summary>
    /// Should the mosaic have a margin?
    /// </summary>
    [Parameter]
    public bool Margin { get; set; } = true;

    /// <summary>
    /// Should the mosaic have a background?
    /// </summary>
    [Parameter]
    public bool Background { get; set; } = true;

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        Classes.Add($"gap-{Display.GapMultiplier}");
        if (Margin)
            Classes.Add($"my-{Display.GapMultiplier * 2}");
        if (Padding)
        {
            Classes.Add($"px-{Display.GapMultiplier}");
            Classes.Add($"py-{Display.GapMultiplier}");
        }
        base.OnParametersSet();
    }
}
