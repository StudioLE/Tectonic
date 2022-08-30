using Lineweights.Workflows.Visualization;

namespace Lineweights.Workflows.NUnit.Visualization;

/// <inheritdoc cref="VisualizeAsFile"/>
public sealed class VisualizeAsFileAfterTestAttribute : VisualizeAfterTestAttribute
{
    /// <inheritdoc />
    public override IVisualizationStrategy? Strategy { get; }

    /// <inheritdoc cref="VisualizeAsFileAfterTestAttribute"/>
    public VisualizeAsFileAfterTestAttribute(params string[] fileExtensions)
    {
        Strategy = new VisualizeAsFile(fileExtensions);
    }
}
