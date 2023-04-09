using Lineweights.Curves;
using Lineweights.Curves.Interpolation;
using Lineweights.Diagnostics;
using Lineweights.Diagnostics.NUnit.Visualization;
using Lineweights.Masterplanning.Elements;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Lineweights.Masterplanning.Tests.Elements;

internal sealed class PathTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    //private readonly IReadOnlyCollection<Vector3> _points = new[]
    //{
    //    new Vector3(0, 0),
    //    //new Vector3(0,5),
    //    new Vector3(1, 7),
    //    //new Vector3(3, 8),
    //    new Vector3(14, 8),
    //    new Vector3(27, 15),
    //    new Vector3(36, 12),
    //    new Vector3(44, 8)
    //};

    //private readonly Vector3 _startTangent = Vector3.YAxis * 2;
    //private readonly Vector3 _endTangent = new Vector3(1, 1).Unitized() * 2;

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
    public async Task Path_Construct()
    {
        // Arrange
        const double width = 0.1;
        Spline centerSpline = new(_points)
        {
            Interpolation = new Cubic(),
            FrameType = FrameType.RoadLike,
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        centerSpline.UpdateRepresentation();

        // Act
        Path path = new(centerSpline, width, 0.005);

        // Preview
        _model.AddElements(new ModelCurve(centerSpline, MaterialByName("Red")));
        _model.AddElements(path);

        // Assert
        await _verify.ElementsByBounds(new[] { path });
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
