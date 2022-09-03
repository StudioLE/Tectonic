using Ardalis.Result;
using Lineweights.Curves.Interpolation;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Curves.Tests;

[VisualizeAfterTest]
internal sealed class SplineTests
{
    private readonly Model _model = new();
    internal static readonly IReadOnlyCollection<Vector3> _points = new[]
    {
        new Vector3(1, 3.5),
        new Vector3(2, 1.25),
        new Vector3(3, 1.5),
        new Vector3(4, 1),
        new Vector3(5, 3.75),
        new Vector3(6, 0),
        new Vector3(7, 2.25),
        new Vector3(8, 0.5),
    };

    internal static readonly Vector3 _startTangent = Vector3.XAxis.Negate() * 2;
    internal static readonly Vector3 _endTangent = Vector3.XAxis * 2;

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
    public void Spline_Construct_Linear()
    {
        // Arrange
        // Act
        var spline = new Spline(_points, new Linear());
        spline.UpdateRepresentation();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Red")));

        // Assert
        Verify.Geometry(spline);
    }

    [Test]
    public void Spline_Construct_Cosine()
    {
        // Arrange
        // Act
        var spline = new Spline(_points)
        {
            Interpolation = new Cosine()
        };
        spline.UpdateRepresentation();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Red")));

        // Assert
        Verify.Geometry(spline);
    }

    [Test]
    public void Spline_Construct_Cubic()
    {
        // Arrange
        // Act
        var spline = new Spline(_points)
        {
            Interpolation = new Cubic(),
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        spline.UpdateRepresentation();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Red")));

        // Assert
        Verify.Geometry(spline);
    }

    [Test]
    public void Spline_Construct_CatmullRom()
    {
        // Arrange
        // Act
        var spline = new Spline(_points)
        {
            Interpolation = new CatmullRom(),
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        spline.UpdateRepresentation();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Red")));

        // Assert
        Verify.Geometry(spline);
    }

    [Test]
    public void Spline_Construct_Hermite()
    {
        // Arrange
        // Act
        var spline = new Spline(_points)
        {
            Interpolation = new Hermite(),
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        spline.UpdateRepresentation();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Red")));

        // Assert
        Verify.Geometry(spline);
    }

    [Test]
    public void Spline_Construct_All()
    {
        // Arrange
        // Act
        var linearSpline = new Spline(_points, new Linear());
        linearSpline.UpdateRepresentation();
        var cosineSpline = new Spline(_points, new Cosine());
        cosineSpline.UpdateRepresentation();
        var cubicSpline = new Spline(_points, new Cubic())
        {
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        cubicSpline.UpdateRepresentation();
        var catmullRomSpline = new Spline(_points, new CatmullRom())
        {
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        catmullRomSpline.UpdateRepresentation();
        var hermiteSpline = new Spline(_points, new Hermite())
        {
            StartTangent = _startTangent,
            EndTangent = _endTangent
        };
        hermiteSpline.UpdateRepresentation();

        // Preview
        _model.AddElements(new ModelCurve(linearSpline, MaterialByName("Red")));
        _model.AddElements(new ModelCurve(cosineSpline, MaterialByName("Green")));
        _model.AddElements(new ModelCurve(cubicSpline, MaterialByName("Orange")));
        _model.AddElements(new ModelCurve(catmullRomSpline, MaterialByName("Magenta")));
        _model.AddElements(new ModelCurve(hermiteSpline, MaterialByName("Blue")));

        // Assert
        Verify.Geometry(linearSpline, cosineSpline, cubicSpline, catmullRomSpline, hermiteSpline);
    }

    [Test]
    public void Spline_RenderVertices()
    {
        // Arrange
        var linearSpline = new Spline(_points);
        var cubicSpline = new Spline(_points, new Cubic());

        // Act
        IReadOnlyCollection<Vector3> linearVertices = linearSpline.Vertices.ToList();
        IReadOnlyCollection<Vector3> cubicVertices = cubicSpline.Vertices.ToList();

        // Preview
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(linearVertices, "Red", "Blue"));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(cubicVertices, "Orange", "Green"));

        // Assert
        Verify.Geometry(linearVertices, cubicVertices);
    }

    [TestCase(FrameType.Frenet)]
    [TestCase(FrameType.RoadLike)]
    public void Spline_TransformAt(FrameType frameType)
    {
        // Arrange
        var spline = new Spline(_points)
        {
            Interpolation = new Cubic(),
            StartTangent = _startTangent,
            EndTangent = _endTangent,
            FrameType = frameType
        };
        spline.UpdateRepresentation();
        var bezier = new Bezier(_points.Select(x => x + Vector3.XAxis * -8).ToList(), frameType);

        // Act
        IReadOnlyCollection<Transform> splineTransforms = Enumerable
            .Range(0, spline.SampleCount + 1)
            .Select(i => spline.TransformAt((double)i / spline.SampleCount))
            .ToArray();
        IReadOnlyCollection<Transform> bezierTransforms = Enumerable
            .Range(0, spline.SampleCount + 1)
            .Select(i => bezier.TransformAt((double)i / spline.SampleCount))
            .ToArray();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Gray")));
        _model.AddElements(new ModelCurve(bezier, MaterialByName("Gray")));
        _model.AddElements(splineTransforms.Select(x => CreateModelArrows.ByTransform(x, 0.1)));
        _model.AddElements(bezierTransforms.Select(x => CreateModelArrows.ByTransform(x, 0.1)));

        // Assert
        Verify.Geometry(splineTransforms, bezierTransforms);
    }

    [TestCase(FrameType.Frenet)]
    [TestCase(FrameType.RoadLike)]
    public void Spline_NormalAt_TangentAt(FrameType frameType)
    {
        // Arrange
        var spline = new Spline(_points)
        {
            Interpolation = new Cubic(),
            StartTangent = _startTangent,
            EndTangent = _endTangent,
            FrameType = frameType
        };
        spline.UpdateRepresentation();
        var bezier = new Bezier(_points.Select(x => x + Vector3.XAxis * -8).ToList(), frameType);
        IReadOnlyCollection<double> uValues = Enumerable
            .Range(0, spline.SampleCount + 1)
            .Select(i => (double)i / spline.SampleCount)
            .ToArray();

        // Act
        IReadOnlyCollection<(Vector3 Point, Vector3 Tangent, Vector3 Normal)> splineResults = uValues
            .Select(u => (spline.PointAt(u), spline.TangentAt(u), spline.NormalAt(u)))
            .ToArray();
        IReadOnlyCollection<(Vector3 Point, Vector3 Tangent, Vector3 Normal)> bezierResults = uValues
            .Select(u => (bezier.PointAt(u), bezier.TangentAt(u), bezier.NormalAt(u)))
            .ToArray();

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Gray")));
        _model.AddElements(new ModelCurve(bezier, MaterialByName("Gray")));
        _model.AddElements(splineResults.Select(x => CreateModelArrows.ByVectors(x.Point, x.Tangent, 0.1, Colors.Blue)));
        _model.AddElements(bezierResults.Select(x => CreateModelArrows.ByVectors(x.Point, x.Tangent, 0.1, Colors.Blue)));
        _model.AddElements(splineResults.Select(x => CreateModelArrows.ByVectors(x.Point, x.Normal, 0.1, Colors.Red)));
        _model.AddElements(bezierResults.Select(x => CreateModelArrows.ByVectors(x.Point, x.Normal, 0.1, Colors.Red)));

        // Assert
        Verify.Geometry(splineResults, bezierResults);
    }

    [TestCase(FrameType.Frenet)]
    [TestCase(FrameType.RoadLike)]
    public void Spline_Offset(FrameType frameType)
    {
        // Arrange
        const double distance = 0.1;
        var spline = new Spline(_points)
        {
            Interpolation = new Cubic(),
            StartTangent = _startTangent,
            EndTangent = _endTangent,
            FrameType = frameType
        };
        spline.UpdateRepresentation();

        // Act
        Spline nearSpline = spline.Offset(distance, true);
        Spline farSpline = spline.Offset(distance, false);

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Gray")));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(nearSpline.Vertices.ToList(), "Red", "Blue"));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(farSpline.Vertices.ToList(), "Orange", "Green"));

        // Assert
        Verify.Geometry(nearSpline, farSpline);
    }

    [TestCase(FrameType.RoadLike)]
    [Explicit("Failing")]
    public void Spline_Offset_WithInvertedSegments(FrameType frameType)
    {
        // Arrange
        const double distance = 0.2;
        var spline = new Spline(_points)
        {
            Interpolation = new Cubic(),
            StartTangent = _startTangent,
            EndTangent = _endTangent,
            FrameType = frameType
        };

        // Act
        Spline nearSpline = spline.Offset(distance, true);
        nearSpline = nearSpline.RemoveIntersections();
        nearSpline = nearSpline.RemoveShortSegments(0.01);
        Spline farSpline = spline.Offset(distance, false);
        farSpline = farSpline.RemoveIntersections();
        farSpline = farSpline.RemoveShortSegments(0.01);

        // Preview
        _model.AddElements(new ModelCurve(spline, MaterialByName("Gray")));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(nearSpline.Vertices.ToList(), "Red", "Blue"));
        _model.AddElements(CreateModelCurve.WithAlternatingMaterials(farSpline.Vertices.ToList(), "Orange", "Green"));

        // Assert
        Verify.Geometry(nearSpline, farSpline);
    }

    [Test]
    public void Spline_Serialisation()
    {
        // Arrange
        var sourceSpline = new Spline(_points)
        {
            Interpolation = new Hermite()
            {
                Tension = 4
            },
            StartTangent = _startTangent,
            EndTangent = _endTangent,
            FrameType = FrameType.RoadLike
        };
        Model model = new();
        model.AddElements(new ModelCurve(sourceSpline, MaterialByName("Gray")));

        // Act
        string json = model.ToJson(true);
        Result<Model> result = ModelHelpers.TryGetFromJson(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, "Serialisation succeeded.");
            Assert.That(result.Errors, Is.Empty, "Serialisation errors.");
            Spline spline = (Spline)model
                .AllElementsOfType<ModelCurve>()
                .First()
                .Curve;
            Assert.That(spline.Interpolation, Is.EqualTo(sourceSpline.Interpolation), "Interpolation type.");
            Assert.That(spline, Is.EqualTo(sourceSpline), "Spline.");
        });
    }
}
