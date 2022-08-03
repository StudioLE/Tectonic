namespace Lineweights.Results.Tests;

[SendToDashboardAfterTest]
internal sealed class ScenesTests : ResultModel
{
    [Test]
    public void Scenes_Brickwork_FromJson()
    {
        // Arrange
        // Act
        ElementInstance[] geometry = Scenes.Brickwork();

        // Preview
        Model.AddElements(geometry);

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
        Model.AddElements(geometry);

        // Assert
        Verify.ElementsByBounds(geometry);
    }
}
