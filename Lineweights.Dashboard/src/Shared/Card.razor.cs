using System.IO;
using Lineweights.Dashboard.Scripts;
using Lineweights.Results;
using Microsoft.AspNetCore.Components;

namespace Lineweights.Dashboard.Shared;

/// <summary>
/// Code-behind the index page.
/// </summary>
public class CardBase : ComponentBase
{
    /// <summary>
    /// Manage the URI navigation.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// The web environment.
    /// </summary>
    [Inject]
    protected IWebHostEnvironment Env { get; set; } = default!;

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
    protected override void OnInitialized()
    {
        foreach (Result child in Result.Children)
            Publish(child);
    }

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
        Result glb = Result
                         .Children
                         .FirstOrDefault(x => x.Metadata.Location?.AbsoluteUri.EndsWith(".glb") ?? false)
                     ?? throw new("Signal didn't contain a .glb");

        await Three.Init(Result.Metadata.Id.ToString(), glb.Metadata.Location!.AbsoluteUri);
    }

    /// <summary>
    /// Publish the contents of the tile.
    /// </summary>
    private void Publish(Result result)
    {
        // Only process files
        if (result.Metadata.Location is null || !result.Metadata.Location.IsFile)
            return;
        FileInfo file = new(result.Metadata.Location.LocalPath);
        if (!file.Exists)
            throw new("File path does not exist.");

        const string relativeTempPath = "temp/";
        FileInfo published = new(Path.Combine(Env.WebRootPath, relativeTempPath, file.Name));
        if (!published.Exists)
            File.Copy(file.FullName, published.FullName);

        result.Metadata.Location = NavigationManager.ToAbsoluteUri(relativeTempPath + file.Name);
    }

    /// <summary>
    /// Hide this tile.
    /// </summary>
    protected void Hide()
    {
        // TODO: Remove from observable
        IsHidden = true;
        //InvokeAsync(StateHasChanged);
    }
}
