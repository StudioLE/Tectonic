using Geometrician.Diagnostics;
using Geometrician.Diagnostics.NUnit.Visualization;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Core.Tests.Geometry;

internal sealed class CreateRuledSurfaceTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [SetUp]
    public void Setup()
    {
        _model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(1, 1, 1)]
    [TestCase(1, 1, .5)]
    public async Task CreateRuledSurface_HyperbolicParaboloidAsLines(double x, double y, double z)
    {
        // Arrange
        Transform transform = new();
        transform.Scale(new Vector3(x, y, z));

        // Act
        IEnumerable<Line> lines = CreateRuledSurface.HyperbolicParaboloidAsLines(10, transform);

        // Preview
        _model.AddElements(lines.Select(_ => new ModelCurve(_)));

        // Assert
        await _verify.Geometry(lines);
    }

    [TestCase(1, 1, 1)]
    public async Task CreateRuledSurface_ConoidAsLines(double x, double y, double z)
    {
        // Arrange
        Transform transform = new();
        transform.Scale(new Vector3(x, y, z));

        // Act
        IEnumerable<Curve> lines = CreateRuledSurface.ConoidAsLines(10, 90, transform);

        // Preview
        _model.AddElements(lines.Select(_ => new ModelCurve(_)));

        // Assert
        await _verify.Geometry(lines);
    }

    [Test]
    public async Task CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid()
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
        _model.AddElements(lines.Select(_ => new ModelCurve(_)));

        // Assert
        await _verify.Geometry(lines);
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
