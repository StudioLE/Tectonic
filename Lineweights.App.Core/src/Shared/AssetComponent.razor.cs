using Lineweights.Workflows.Assets;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.App.Core.Shared;

public class AssetComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<AssetComponent> Logger { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Asset Asset { get; set; } = default!;

    /// <summary>
    /// Should the card be hidden?
    /// </summary>
    protected bool IsHidden { get; set; } = false;

    /// <summary>
    /// Hide this tile.
    /// </summary>
    protected void Hide()
    {
        Logger.LogDebug($"{nameof(Hide)}() called on result {Asset.Info.Id}.");

        // TODO: Instead of this remove it from the observable collection
        IsHidden = true;

        //InvokeAsync(StateHasChanged);
    }

    public string GetTitle()
    {
        return string.IsNullOrEmpty(Asset.Info.Name)
            ? Asset.Info.Id.ToString()
            : Asset.Info.Name;
    }
}
