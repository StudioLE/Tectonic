using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Visualization;

public class OutcomeComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<OutcomeComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    protected VisualizationState State { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Outcome Outcome { get; set; } = default!;

    /// <summary>
    /// Remove the asset from the collection.
    /// </summary>
    protected void Remove()
    {
        Logger.LogDebug($"{nameof(Remove)}() called on result {Outcome.Id}.");
        State.RemoveOutcome(Outcome);
    }

    public string GetTitle()
    {
        return string.IsNullOrEmpty(Outcome.Name)
            ? Outcome.Id.ToString()
            : Outcome.Name;
    }
}
