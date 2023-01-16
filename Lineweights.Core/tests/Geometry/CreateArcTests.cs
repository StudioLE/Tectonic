using Lineweights.Diagnostics.NUnit.Visualization;

namespace Lineweights.Core.Tests.Geometry;

internal sealed class CreateArcTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [TestCase(1, 0, 0, 1, 0, 0)]
    [TestCase(1, 0, 0, 1, 2, 2)]
    [TestCase(-3, 4, 4, 5, 1, -4)]
    public async Task Create_Arc_By3Points(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        // Arrange
        Vector3 start = new(x1, y1);
        Vector3 end = new(x2, y2);
        Vector3 pointOnArc = new(x3, y3);
        ModelCurve ab = new(new Line(start, pointOnArc), MaterialByName("Red"));
        ModelCurve bc = new(new Line(pointOnArc, end), MaterialByName("Blue"));

        // Act
        Arc arc = CreateArc.ByThreePoints(start, end, pointOnArc);

        // Preview
        _model.AddElements(ab, bc, new ModelCurve(arc, MaterialByName("Orange")));

        // Assert
        Assert.Multiple(() =>
        {
            const double threshold = 1e-2;
            Assert.That(start.IsPointOn(arc, threshold: threshold), Is.True);
            Assert.That(end.IsPointOn(arc, threshold: threshold), Is.True);
            Assert.That(pointOnArc.IsPointOn(arc, threshold: threshold), Is.True);
        });
        await Verify.Geometry(arc);
    }

    [TestCase(3, 2, 2, 4, 3, 6, 7, 8)]
    public async Task Create_Arc_ByPolyline(
        double x0, double y0,
        double x1, double y1,
        double x2, double y2,
        double x3, double y3)
    {
        // Arrange
        Vector3[] vertices =
        {
            new Vector3(x0, y0),
            new Vector3(x1, y1),
            new Vector3(x2, y2),
            new Vector3(x3, y3)
        };
        Polyline polyline = new(vertices);

        // Act
        Arc arc = CreateArc.ByPolyline(polyline);

        // Preview
        _model.AddElements(polyline, new ModelCurve(arc, MaterialByName("Orange")));

        // Assert
        Assert.Multiple(() =>
        {
            const double threshold = 1e-2;
            Assert.That(vertices[0].IsPointOn(arc, threshold: threshold), Is.True);
            //Assert.That(vertices[1].IsPointOn(arc, threshold), Is.True);
            //Assert.That(vertices[2].IsPointOn(arc, threshold), Is.True);
            Assert.That(vertices[3].IsPointOn(arc, threshold: threshold), Is.True);
        });
        await Verify.Geometry(arc);
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
