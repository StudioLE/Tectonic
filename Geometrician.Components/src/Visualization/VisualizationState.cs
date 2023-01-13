﻿using System.Collections.ObjectModel;
using Geometrician.Core.Assets;

namespace Geometrician.Components.Visualization;

/// <summary>
/// The current state of the activity execution outcomes.
/// </summary>
public class VisualizationState
{
    private readonly AssetFactoryProvider _provider;
    private readonly Collection<Outcome> _outcomes = new();

    /// <inheritdoc cref="VisualizationState"/>
    public VisualizationState(AssetFactoryProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// The <see cref="Outcome"/> currently in state.
    /// </summary>
    public IReadOnlyCollection<Outcome> Outcomes => _outcomes;

    /// <summary>
    /// An event handler which emits when <see cref="Outcomes"/> is changed.
    /// </summary>
    public event EventHandler? OutcomesChanged;

    /// <summary>
    /// Add an <see cref="Outcome"/> to <see cref="Outcomes"/>.
    /// </summary>
    /// <param name="outcome">The outcome to add.</param>
    /// <param name="output">The output of the execution from which assets will be derived.</param>
    public void AddOutcome(Outcome outcome, object output)
    {
        outcome.Assets = _provider.GetFactoriesForObjectProperties(output);
        _outcomes.Add(outcome);
        OutcomesChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Remove an <see cref="Outcome"/> from <see cref="Outcomes"/>.
    /// </summary>
    /// <param name="outcome">The outcome to remove.</param>
    /// <returns>Whether the outcome was removed.</returns>
    public bool RemoveOutcome(Outcome outcome)
    {
        bool wasRemoved = _outcomes.Remove(outcome);
        if (wasRemoved)
            OutcomesChanged?.Invoke(this, EventArgs.Empty);
        return wasRemoved;
    }
}
