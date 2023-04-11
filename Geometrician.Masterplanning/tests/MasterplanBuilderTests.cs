using Geometrician.Curves;
using Geometrician.Curves.Interpolation;
using Geometrician.Diagnostics;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Masterplanning.Elements;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Masterplanning.Tests;

internal sealed class MasterplanBuilderTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private readonly IReadOnlyCollection<Vector3> _points = new[]
    {
        new Vector3(1, 3.5),
        new Vector3(2, 1.25),
        new Vector3(3, 1.5),
        new Vector3(4, 1),
        new Vector3(5, 3.75),
        new Vector3(6, 0),
        new Vector3(7, 2.25),
        new Vector3(8, 0.5)
    };

    private readonly Vector3 _startTangent = Vector3.XAxis.Negate() * 2;
    private readonly Vector3 _endTangent = Vector3.XAxis * 2;
    private Model _model = new();

    [SetUp]
    public void Setup()
    {
        _model.AddElements(
            new ModelCurve(
                new Line(
                    new(0, 0),
                    new(0, 8)),
                MaterialByName("Gray")),
            new ModelCurve(
                new Line(
                    new(0, 0),
                    new(8, 0)),
                MaterialByName("Gray")));
    }

    [Test]
    public async Task MasterplanBuilder_Build()
    {
        // Arrange
        Spline centerSpline = new(_points)
        {
            Interpolation = new Cubic(),
            FrameType = FrameType.RoadLike,
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        centerSpline.UpdateRepresentation();
        MasterplanBuilder builder = new()
        {
            MajorSplines = new[]
            {
                centerSpline
            }
        };

        // Act
        IReadOnlyCollection<Path> results = builder.Build();

        // Preview
        _model.AddElements(new ModelCurve(centerSpline, MaterialByName("Red")));
        _model.AddElements(results);

        // Assert
        await _verify.ElementsByBounds(results);
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
