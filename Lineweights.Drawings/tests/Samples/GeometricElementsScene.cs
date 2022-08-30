using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Drawings.Tests.Samples;

[VisualizeInServerAppAfterTest]
internal sealed class GeometricElementsScene
{
    public Model Model { get; } = new();

    [Test]
    [Explicit("Write GeometricElementsScene to JSON")]
    public void GeometricElementsScene_ToJSON()
    {
        // Arrange
        // Act
        GeometricElement[] geometry = Enumerable.Empty<GeometricElement>()
            .Append(OriginAsModelArrows())
            .Concat(BoxAsModelCurves())
            .Append(CircleAsModelCurves())
            .Append(ExtrudeAsGeometricElement())
            .Append(LaminaAsPanel())
            .Append(SweepAsBeam())
            .Concat(RuledSurfaceAsModelCurves())
            .Append(SphereAsMeshElement())
            .ToArray();

        // Preview
        Model.AddElements(geometry);

        // Write to file
        Scenes.ToJson(Scenes.Name.GeometricElements, geometry);
    }

    private static ModelArrows OriginAsModelArrows()
    {
        return CreateModelArrows.ByTransform(new(-.400, -.400, -.400), .200);
    }

    private static IEnumerable<ModelCurve> BoxAsModelCurves()
    {
        return new Box(CreateBBox3.ByLengths(1, 1, 1)).ToModelCurves(MaterialByName("Navy"));
    }

    private static ModelCurve CircleAsModelCurves()
    {
        return new(new Circle(.200), MaterialByName("Purple"));
    }

    private static GeometricElement ExtrudeAsGeometricElement()
    {
        Transform transform = new();
        transform.Rotate(Vector3.ZAxis, 45);
        transform.Rotate(Vector3.XAxis, -45);

        Extrude extrude = CreateBBox3.ByLengths(.500, .200, .100).ToExtrude();
        return new(transform, MaterialByName("Green"), extrude);
    }

    private static Panel LaminaAsPanel()
    {
        Polygon polygon = new(
            new(.400, .400, -.250),
            new(.200, .200, -.250),
            new(.200, .200, .350),
            new(.300, .300, .350)
        );
        return new(polygon, MaterialByName("Cobalt"));
    }

    private static Beam SweepAsBeam()
    {
        Bezier bezier = new(new()
        {
            new(.100, -.100, -.450),
            new(.100, -.100, -.200),
            new(.250, -.250, -.100),
            new(.250, -.250, .100)
        });
        return new(bezier, Polygon.Rectangle(.050, .100), MaterialByName("Cyan"));
    }

    private static MeshElement SphereAsMeshElement()
    {
        return new(Mesh.Sphere(.150), MaterialByName("Lime"), new(.350, -.350, .350));
    }

    private static IEnumerable<ModelCurve> RuledSurfaceAsModelCurves()
    {
        IList<Vector3> topVertices = Polygon
            .Ngon(120, .500)
            .Vertices;
        IList<Vector3> bottomVertices = Polygon
            .Ngon(120, .100)
            .TransformedPolyline(new(0, 0, -.500))
            .Vertices;

        Polyline top = new(topVertices
            .Skip(30)
            .Take(31)
            .ToArray());
        Polyline bottomA = new(bottomVertices
            .Skip(15)
            .Take(31)
            .ToArray());
        Polyline bottomB = new(bottomVertices
            .Skip(45)
            .Take(31)
            .ToArray());

        IEnumerable<ModelCurve> modelCurvesA = CreateModelCurve.WithAlternatingMaterials(
            CreateRuledSurface.AsLinesByCurves(top, bottomA, 10),
            MaterialByName("Orange"),
            MaterialByName("Aqua"));
        IEnumerable<ModelCurve> modelCurvesB = CreateModelCurve.WithAlternatingMaterials(
            CreateRuledSurface.AsLinesByCurves(top, bottomB, 10),
            MaterialByName("Red"),
            MaterialByName("Blue"));

        return Enumerable.Empty<ModelCurve>()
            .Append(new(top, MaterialByName("Gray")))
            .Append(new(bottomA, MaterialByName("Gray")))
            .Append(new(bottomB, MaterialByName("Gray")))
            .Concat(modelCurvesA)
            .Concat(modelCurvesB);
    }
}
