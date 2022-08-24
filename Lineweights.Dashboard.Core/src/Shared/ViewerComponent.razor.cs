using Lineweights.Dashboard.Core.Scripts;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Shared;

public partial class ViewerComponent
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<ViewerComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="ModelViewerFacade"/>
    [Inject]
    protected ModelViewerFacade Three { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Result Result { get; set; } = default!;

    protected Guid ComponentId { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Result.IsFileType(".glb"))
            await LoadGlb();
    }

    /// <summary>
    /// Load the GLB with Three.js
    /// </summary>
    private async Task LoadGlb()
    {
        Logger.LogDebug($"{nameof(LoadGlb)}() called on result {Result.Info.Id}.");
        await Three.Init(ComponentId.ToString(), Result.Info.Location!.AbsoluteUri);
    }
}
