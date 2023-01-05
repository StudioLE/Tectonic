using Lineweights.Core.Documents;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Workflows.Tests;

internal sealed class SamplesTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();
    private IReadOnlyCollection<Asset> _assets = Array.Empty<Asset>();

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
        _assets = outputs.Assets;

        // Assert
        await Verify.ElementsByBounds(outputs.Model.Elements.Values);
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model, _assets);
        _model = new();
        _assets = Array.Empty<Asset>();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
