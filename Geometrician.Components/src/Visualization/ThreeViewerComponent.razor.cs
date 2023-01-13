using Geometrician.Components.Scripts;
using Lineweights.Core.Assets;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StudioLE.Core.Results;

namespace Geometrician.Components.Visualization;

/// <summary>
/// A <see cref="IComponent"/> to render an <see cref="IAsset"/> using the Three.js viewer.
/// </summary>
public class ThreeViewerComponentBase : ViewerComponentBase<ExternalAsset>
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ThreeViewerComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="ModelViewer"/>
    [Inject]
    private ModelViewer Three { get; set; } = default!;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if(firstRender && Factory.Result is Success)
            await LoadGlb();
    }

    /// <inheritdoc />
    protected override async Task AfterExecution()
    {
        await InvokeAsync(StateHasChanged);
        await LoadGlb();
    }

    /// <summary>
    /// Load the GLB with Three.js.
    /// </summary>
    private async Task LoadGlb()
    {
        Logger.LogDebug($"{nameof(LoadGlb)}() called on result {Factory.Asset.Id}.");
        if (Factory.Asset.Location is null)
        {
            Logger.LogWarning("Factory.Result.Location is not set.");
            return;
        }
        await Three.Create(Factory.Asset.Id.ToString(), Factory.Asset.Location.AbsoluteUri);
    }
}
