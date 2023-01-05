using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Workflows.Tests;

internal sealed class SamplesTests
{
    private readonly Model _model = new();

    [TestCase(Scenes.Name.Brickwork)]
    [TestCase(Scenes.Name.GeometricElements)]
    public async Task AssetTypes(Scenes.Name name)
    {
        // Arrange
        AssetTypes.Inputs inputs = new()
        {
            Scene = name
        };

        // Act
        AssetTypes.Outputs outputs = Samples.AssetTypes.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);
        await new Visualize().Execute(_model, outputs.Assets);

        // Assert
        await Verify.ElementsByBounds(outputs.Model.Elements.Values);
    }
}
