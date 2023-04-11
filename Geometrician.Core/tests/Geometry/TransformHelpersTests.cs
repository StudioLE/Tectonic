using Geometrician.Diagnostics.NUnit.Visualization;

namespace Geometrician.Core.Tests.Geometry;

internal sealed class TransformHelpersTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public void TransformHelpers_RotationBetween()
    {
        // Arrange
        Vector3 sourceOrigin = new(1, 1, 1);
        Transform source = new(sourceOrigin, Vector3.YAxis, Vector3.ZAxis, Vector3.XAxis);
        Transform target = new();

        // Act
        Transform rotation = TransformHelpers.RotationBetween(source, target);
        Transform rotated = source.Concatenated(rotation);

        // Preview
        _model.AddElements(CreateModelArrows.ByTransform(rotated));
        _model.AddElements(CreateModelArrows.ByTransform(target));

        Transform rounded = rotated.RoundedAxis(5);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rounded.XAxis, Is.EqualTo(target.XAxis), "X Axis");
            Assert.That(rounded.YAxis, Is.EqualTo(target.YAxis), "Y Axis");
            Assert.That(rounded.ZAxis, Is.EqualTo(target.ZAxis), "Z Axis");
            Assert.That(rounded.Origin, Is.EqualTo(sourceOrigin), "Origin");
        });
    }

    [Test]
    public void TransformHelpers_RotateTo()
    {
        // Arrange
        Vector3 sourceOrigin = new(1, 1, 1);
        Transform source = new(sourceOrigin, Vector3.YAxis, Vector3.ZAxis, Vector3.XAxis);
        Transform target = new();

        // Act
        source.RotateTo(target);

        // Preview
        _model.AddElements(CreateModelArrows.ByTransform(source));
        _model.AddElements(CreateModelArrows.ByTransform(target));

        Transform rounded = source.RoundedAxis(5);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rounded.XAxis, Is.EqualTo(target.XAxis), "X Axis");
            Assert.That(rounded.YAxis, Is.EqualTo(target.YAxis), "Y Axis");
            Assert.That(rounded.ZAxis, Is.EqualTo(target.ZAxis), "Z Axis");
            Assert.That(rounded.Origin, Is.EqualTo(sourceOrigin), "Origin");
        });
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
