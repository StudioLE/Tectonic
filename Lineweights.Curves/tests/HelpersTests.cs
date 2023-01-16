using Lineweights.Curves.Interpolation;
using Lineweights.Diagnostics.NUnit.Visualization;

namespace Lineweights.Curves.Tests;

internal sealed class HelpersTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [TestCase(0.1)]
    [TestCase(0.5)]
    [TestCase(1)]
    public async Task Helpers_CombineSegmentsToArcs_Polyline(double angleThreshold)
    {
        // Arrange
        Polyline polyline = new(
            new Vector3(0, 0),
            new Vector3(0, 3),
            new Vector3(2, 5),
            new Vector3(6, 5),
            new Vector3(9, 7));

        // Act
        IReadOnlyCollection<Curve> arcs = Helpers.CombineSegmentsToArcs(polyline, angleThreshold);

        // Preview
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(arcs, "Red", "Blue"));

        // Assert
        await Verify.Geometry(arcs);
    }

    [TestCase(0.1)]
    [TestCase(0.5)]
    [TestCase(1)]
    public async Task Helpers_CombineSegmentsToArcs_Spline(double angleThreshold)
    {
        // Arrange
        Spline spline = new(SplineTests._points)
        {
            Interpolation = new Cubic(),
            StartTangent = SplineTests._startTangent,
            EndTangent = SplineTests._endTangent,
            FrameType = FrameType.RoadLike
        };
        spline.UpdateRepresentation();

        // Act
        IReadOnlyCollection<Curve> arcs = Helpers.CombineSegmentsToArcs(spline, angleThreshold);

        // Preview
        _model.AddElements(new ModelCurve(new Polyline(spline.KeyVertices.ToList()), MaterialByName("Gray")));
        _model.AddElements(new ModelCurve(spline, MaterialByName("Gray")));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(arcs, "Red", "Blue"));

        // Assert
        await Verify.Geometry(arcs);
    }

    [TestCase(0.1)]
    [TestCase(0.5)]
    [TestCase(1)]
    public async Task Helpers_CombineSegmentsToArcs_Spline_PreserveKeyVertices(double angleThreshold)
    {
        // Arrange
        Spline spline = new(SplineTests._points)
        {
            Interpolation = new Cubic(),
            StartTangent = SplineTests._startTangent,
            EndTangent = SplineTests._endTangent,
            FrameType = FrameType.RoadLike
        };
        spline.UpdateRepresentation();

        // Act
        IReadOnlyCollection<Curve> arcs = Helpers.CombineSegmentsToArcs(spline, angleThreshold, true);

        // Preview
        _model.AddElements(new ModelCurve(new Polyline(spline.KeyVertices.ToList()), MaterialByName("Gray")));
        _model.AddElements(new ModelCurve(spline, MaterialByName("Gray")));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(arcs, "Red", "Blue"));

        // Assert
        await Verify.Geometry(arcs);
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
