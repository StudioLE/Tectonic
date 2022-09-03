using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Workflows.Tests;

[VisualizeAfterTest]
internal sealed class ScenesTests
{
    private readonly Model _model = new();

    [Test]
    public void Scenes_Brickwork_FromJson()
    {
        // Arrange
        // Act
        ElementInstance[] geometry = Scenes.Brickwork();

        // Preview
        _model.AddElements(geometry);

        // Assert
        Verify.ElementsByBounds(geometry);
    }

    [Test]
    public void Scenes_GeometricElements_FromJson()
    {
        // Arrange
        // Act
        GeometricElement[] geometry = Scenes.GeometricElements();

        // Preview
        _model.AddElements(geometry);

        // Assert
        Verify.ElementsByBounds(geometry);
    }
}
