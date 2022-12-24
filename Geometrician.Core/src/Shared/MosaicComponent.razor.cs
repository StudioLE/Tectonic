using Microsoft.AspNetCore.Components;

namespace Geometrician.Core.Shared;

public class MosaicComponentBase : TemplatedComponentBase
{
    /// <inheritdoc cref="Display"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    [Parameter]
    public bool Padding { get; set; } = true;

    [Parameter]
    public bool Margin { get; set; } = true;

    [Parameter]
    public bool Background { get; set; } = true;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Classes.Add($"gap-{Display.GapMultiplier}");
        if(Margin)
            Classes.Add($"my-{Display.GapMultiplier * 2}");
        if (Padding)
        {
            Classes.Add($"px-{Display.GapMultiplier}");
            Classes.Add($"py-{Display.GapMultiplier}");
        }

        base.OnParametersSet();
    }
}
