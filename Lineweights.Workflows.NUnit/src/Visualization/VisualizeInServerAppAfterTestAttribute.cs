using Lineweights.Workflows.Visualization;

namespace Lineweights.Workflows.NUnit.Visualization;

/// <summary>
/// Send the model to the server to be sampled.
/// </summary>
public sealed class VisualizeInServerAppAfterTestAttribute : VisualizeAfterTestAttribute
{
    /// <inheritdoc />
    public override IVisualizationStrategy? Strategy { get; }

    /// <inheritdoc cref="VisualizeInServerAppAfterTestAttribute"/>
    public VisualizeInServerAppAfterTestAttribute()
    {
        Strategy = new VisualizeInServerApp();
    }
}
