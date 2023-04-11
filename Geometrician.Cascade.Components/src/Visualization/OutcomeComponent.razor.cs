using Geometrician.Cascade.Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Cascade.Components.Visualization;

/// <summary>
/// A <see cref="IComponent"/> to render an <see cref="Outcome"/> in the UI.
/// The assets of the <see cref="Outcome"/> are rendered as <see cref="TileComponent"/> inside a <see cref="MosaicComponent"/>.
/// </summary>
public class OutcomeComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<OutcomeComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    private VisualizationState Visualization { get; set; } = default!;

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
        Visualization.RemoveOutcome(Outcome);
    }

    /// <summary>
    /// The title of the outcome.
    /// </summary>
    /// <returns>The title of the outcome.</returns>
    protected string GetTitle()
    {
        return string.IsNullOrEmpty(Outcome.Name)
            ? Outcome.Id.ToString()
            : Outcome.Name;
    }
}
