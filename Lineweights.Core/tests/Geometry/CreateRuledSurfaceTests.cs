using Lineweights.Workflows.Results;

namespace Lineweights.Core.Tests.Geometry;

[SendToDashboardAfterTest]
internal sealed class CreateRuledSurfaceTests : ResultModel
{
    [SetUp]
    public void Setup()
    {
        Model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(1, 1, 1)]
    [TestCase(1, 1, .5)]
    public void CreateRuledSurface_HyperbolicParaboloidAsLines(double x, double y, double z)
    {
        // Arrange
        Transform transform = new();
        transform.Scale(new Vector3(x, y, z));

        // Act
        IEnumerable<Line> lines = CreateRuledSurface.HyperbolicParaboloidAsLines(10, transform);

        // Preview
        Model.AddElements(lines.Select(_ => new ModelCurve(_)));

        // Assert
        Verify.Geometry(lines);
    }

    [TestCase(1, 1, 1)]
    public void CreateRuledSurface_ConoidAsLines(double x, double y, double z)
    {
        // Arrange
        Transform transform = new();
        transform.Scale(new Vector3(x, y, z));

        // Act
        IEnumerable<Curve> lines = CreateRuledSurface.ConoidAsLines(10, 90, transform);

        // Preview
        Model.AddElements(lines.Select(_ => new ModelCurve(_)));

        // Assert
        Verify.Geometry(lines);
    }

    [Test]
    public void CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid()
    {
        // Arrange
        Transform transform = new();
        Vector3[] vertices =
        {
            transform.OfPoint(new(-.5, -.5, 0)),
            transform.OfPoint(new(.5, -.5, 0)),
            transform.OfPoint(new(.5, .5, -.5)),
            transform.OfPoint(new(-.5, .5, .5))
        };
        // Act
        IEnumerable<Line> lines = Enumerable.Empty<Line>()
            .Concat(CreateRuledSurface.AsLinesByCurves(
                new Line(vertices[0], vertices[1]),
                new Line(vertices[3], vertices[2]),
                10))
            .Concat(CreateRuledSurface.AsLinesByCurves(
                new Line(vertices[0], vertices[3]),
                new Line(vertices[1], vertices[2]),
                10));

        // Preview
        Model.AddElements(lines.Select(_ => new ModelCurve(_)));

        // Assert
        Verify.Geometry(lines);
    }
}
