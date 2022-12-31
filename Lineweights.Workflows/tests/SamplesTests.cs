using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Workflows.Tests;

[VisualizeAfterTest]
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
            Scene = name,
            IncludeViewsInModel = false,
            IncludeCsvFileAsAsset = false,
            IncludeIfcFileAsAsset = false,
            IncludeJsonAsContentAsset = false
        };

        // Act
        AssetTypes.Outputs outputs = Samples.AssetTypes.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        await Verify.ElementsByBounds(outputs.Model.Elements.Values);
    }
}
