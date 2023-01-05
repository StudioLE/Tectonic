namespace Lineweights.Workflows.Visualization;

/// <summary>
/// A strategy to visualise or sample a <see cref="Model"/>.
/// </summary>
public interface IVisualizationStrategy
{
    /// <inheritdoc cref="IVisualizationStrategy"/>
    public Task Execute(params VisualizeRequest[] requests);
}
