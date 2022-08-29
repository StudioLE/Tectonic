using Lineweights.Workflows.Results;

namespace Lineweights.Flex.Tests;

[SendToServerAfterTest]
internal sealed class WallSamples : ResultModel
{
    [Test]
    public void Wall_StretcherBond()
    {
        // Arrange
        Samples.WallStretcherBond.Inputs inputs = new();

        // Act
        Samples.WallStretcherBond.Outputs outputs = Samples.WallStretcherBond.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }

    [Test]
    public void Wall_FlemishBond()
    {
        // Arrange
        Samples.WallFlemishBond.Inputs inputs = new();

        // Act
        Samples.WallFlemishBond.Outputs outputs = Samples.WallFlemishBond.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }
}
