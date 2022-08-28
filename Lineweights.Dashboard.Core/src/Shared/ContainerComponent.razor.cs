using Lineweights.Workflows.Containers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Shared;

public class ContainerComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<ContainerComponent> Logger { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Container Container { get; set; } = default!;

    /// <summary>
    /// Should the card be hidden?
    /// </summary>
    protected bool IsHidden { get; set; } = false;

    /// <summary>
    /// Hide this tile.
    /// </summary>
    protected void Hide()
    {
        Logger.LogDebug($"{nameof(Hide)}() called on result {Container.Info.Id}.");

        // TODO: Instead of this remove it from the observable collection
        IsHidden = true;

        //InvokeAsync(StateHasChanged);
    }

    public string GetTitle()
    {
        return string.IsNullOrEmpty(Container.Info.Name)
            ? Container.Info.Id.ToString()
            : Container.Info.Name;
    }
}
