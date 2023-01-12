using System.Collections.ObjectModel;
using Geometrician.Core.Assets;

namespace Geometrician.Components.Visualization;

/// <summary>
/// The global state of the application.
/// </summary>
public class VisualizationState
{
    private readonly AssetFactoryProvider _provider;
    private readonly Collection<Outcome> _outcomes = new();

    public VisualizationState(AssetFactoryProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// The <see cref="Outcome"/> currently in state.
    /// </summary>
    public IReadOnlyCollection<Outcome> Outcomes => _outcomes;

    /// <summary>
    /// The <see cref="Outcome"/> currently in state.
    /// </summary>
    public event EventHandler? OutcomesChanged;

    public void AddOutcome(Outcome outcome, object obj)
    {
        outcome.Assets = _provider.GetFactoriesForObjectProperties(obj);
        _outcomes.Add(outcome);
        OutcomesChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool RemoveOutcome(Outcome outcome)
    {
        bool wasRemoved = _outcomes.Remove(outcome);
        if (wasRemoved)
            OutcomesChanged?.Invoke(this, EventArgs.Empty);
        return wasRemoved;
    }
}
