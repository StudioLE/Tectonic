using Lineweights.Core.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Shared;

public class AssetComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<AssetComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="AssetState"/>
    [Inject]
    protected AssetState State { get; set; } = default!;

    /// <inheritdoc cref="Display"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Asset Asset { get; set; } = default!;

    /// <summary>
    /// Remove the asset from the collection.
    /// </summary>
    protected void Remove()
    {
        Logger.LogDebug($"{nameof(Remove)}() called on result {Asset.Info.Id}.");

        State.Assets.Remove(Asset);

        //InvokeAsync(StateHasChanged);
    }

    public string GetTitle()
    {
        return string.IsNullOrEmpty(Asset.Info.Name)
            ? Asset.Info.Id.ToString()
            : Asset.Info.Name;
    }
}
