using Lineweights.Workflows.Results;

namespace Lineweights.Core.Tests.Geometry;

[SendToServerAfterTest]
internal sealed class TransformHelpersTests : ResultModel
{
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
        Model.AddElements(CreateModelArrows.ByTransform(rotated));
        Model.AddElements(CreateModelArrows.ByTransform(target));

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
        Model.AddElements(CreateModelArrows.ByTransform(source));
        Model.AddElements(CreateModelArrows.ByTransform(target));

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
}
