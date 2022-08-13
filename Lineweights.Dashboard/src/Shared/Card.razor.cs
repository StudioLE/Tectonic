using Lineweights.Dashboard.Scripts;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;

namespace Lineweights.Dashboard.Shared;

/// <summary>
/// Code-behind the index page.
/// </summary>
public class CardBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<CardBase> Logger { get; set; } = default!;

    /// <inheritdoc cref="ModelViewerFacade"/>
    [Inject]
    protected ModelViewerFacade Three { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Result Result { get; set; } = default!;

    /// <summary>
    /// Should the card be hidden?
    /// </summary>
    protected bool IsHidden { get; set; } = false;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await LoadGlb();
        RefreshAtIntervals();
    }

    /// <summary>
    /// Refresh this card at intervals to ensure the time since creation increases.
    /// </summary>
    private void RefreshAtIntervals()
    {
        TimeSpan interval = TimeSpan.FromSeconds(10);
        Timer timer = new(_ => InvokeAsync(StateHasChanged), null, interval, interval);
    }

    /// <summary>
    /// Load the GLB with Three.js
    /// </summary>
    private async Task LoadGlb()
    {
        Logger.LogDebug($"{nameof(LoadGlb)}() called on result {Result.Info.Id}.");
        Result? glb = Result
            .Children
            .FirstOrDefault(x => x.Info.Location?.AbsoluteUri.EndsWith(".glb") ?? false);
        if (glb is null)
        {
            Logger.LogError($"Failed to load glb. Result {Result.Info.Id} did not contain a glb.");
            return;
        }

        await Three.Init(Result.Info.Id.ToString(), glb.Info.Location!.AbsoluteUri);
    }

    /// <summary>
    /// Hide this tile.
    /// </summary>
    protected void Hide()
    {
        Logger.LogDebug($"{nameof(Hide)}() called on result {Result.Info.Id}.");
        // TODO: Instead of this remove it from the observable collection
        IsHidden = true;
        //InvokeAsync(StateHasChanged);
    }
}
