using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

[VisualizeAfterTest]
internal sealed class WallSamples
{
    public Model Model { get; } = new();

    [Test]
    public async Task Wall_StretcherBond()
    {
        // Arrange
        Samples.WallStretcherBond.Inputs inputs = new();

        // Act
        Samples.WallStretcherBond.Outputs outputs = Samples.WallStretcherBond.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }

    [Test]
    public async Task Wall_FlemishBond()
    {
        // Arrange
        Samples.WallFlemishBond.Inputs inputs = new();

        // Act
        Samples.WallFlemishBond.Outputs outputs = Samples.WallFlemishBond.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }
}
