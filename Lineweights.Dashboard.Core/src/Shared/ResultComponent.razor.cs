using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Shared;

public class ResultComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<ResultComponent> Logger { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Result Result { get; set; } = default!;

    /// <summary>
    /// Should the card be hidden?
    /// </summary>
    protected bool IsHidden { get; set; } = false;

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
