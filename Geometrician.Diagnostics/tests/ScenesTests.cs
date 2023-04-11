using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Diagnostics.Samples;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Diagnostics.Tests;

internal sealed class ScenesTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public async Task Scenes_Brickwork_FromJson()
    {
        // Arrange
        // Act
        ElementInstance[] geometry = Scenes.Brickwork();

        // Preview
        _model.AddElements(geometry);

        // Assert
        await _verify.ElementsByBounds(geometry);
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
        await _verify.ElementsByBounds(geometry);
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model);
        _model = new();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
