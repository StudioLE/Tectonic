using Lineweights.Curves.Interpolation;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Curves.Tests;

[VisualizeAfterTest]
internal sealed class HelpersTests
{
    private readonly Model _model = new();

    [TestCase(0.1)]
    [TestCase(0.5)]
    [TestCase(1)]
    public void Helpers_CombineSegmentsToArcs_Polyline(double angleThreshold)
    {
        // Arrange
        var polyline = new Polyline(
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
        Verify.Geometry(arcs);
    }

    [TestCase(0.1)]
    [TestCase(0.5)]
    [TestCase(1)]
    public void Helpers_CombineSegmentsToArcs_Spline(double angleThreshold)
    {
        // Arrange
        var spline = new Spline(SplineTests._points)
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
        Verify.Geometry(arcs);
    }

    [TestCase(0.1)]
    [TestCase(0.5)]
    [TestCase(1)]
    public void Helpers_CombineSegmentsToArcs_Spline_PreserveKeyVertices(double angleThreshold)
    {
        // Arrange
        var spline = new Spline(SplineTests._points)
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
        Verify.Geometry(arcs);
    }
}
