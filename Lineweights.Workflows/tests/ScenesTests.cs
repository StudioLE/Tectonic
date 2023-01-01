using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Workflows.Tests;

internal sealed class ScenesTests
{
    private readonly Model _model = new();

    [Test]
    public async Task Scenes_Brickwork_FromJson()
    {
        // Arrange
        // Act
        ElementInstance[] geometry = Scenes.Brickwork();

        // Preview
        _model.AddElements(geometry);

        // Assert
        await Verify.ElementsByBounds(geometry);
    }

    [Test]
    public async Task Scenes_GeometricElements_FromJson()
    {
        // Arrange
        // Act
        GeometricElement[] geometry = Scenes.GeometricElements();

        // Preview
        _model.AddElements(geometry);

        // Assert
        await Verify.ElementsByBounds(geometry);
    }

    [TearDown]
    public async Task TearDown()
    {
        await new Visualize().Execute(_model);
    }
}
