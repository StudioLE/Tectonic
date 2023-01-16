using Lineweights.Diagnostics.NUnit.Visualization;

namespace Lineweights.Core.Tests.Geometry;

internal sealed class CreateCircleTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [TestCase(1, 0, 0, 1, 0, 0)]
    [TestCase(1, 0, 0, 1, 2, 2)]
    [TestCase(-3, 4, 4, 5, 1, -4)]
    public async Task Create_Circle(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        // Arrange
        Vector3 start = new(x1, y1);
        Vector3 end = new(x2, y2);
        Vector3 pointOnCircle = new(x3, y3);
        ModelCurve ab = new(new Line(start, pointOnCircle), MaterialByName("Red"));
        ModelCurve bc = new(new Line(pointOnCircle, end), MaterialByName("Blue"));

        // Act
        Circle circle = CreateCircle.ByThreePoints(start, end, pointOnCircle);

        // Preview
        _model.AddElements(ab, bc, new ModelCurve(circle, MaterialByName("Orange")));

        // Assert
        await Verify.Geometry(circle);
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
