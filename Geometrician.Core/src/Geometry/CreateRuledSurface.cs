namespace Geometrician.Core.Geometry;

/// <summary>
/// Methods to create ruled surfaces.
/// </summary>
public static class CreateRuledSurface
{
    /// <summary>
    /// Create a ruled surface as lines between two curves.
    /// </summary>
    public static IEnumerable<Line> AsLinesByCurves(
        Curve first,
        Curve second,
        int divisions)
    {
        return Enumerable
            .Range(0, divisions + 1)
            .Select(index =>
            {
                double u = (double)index / divisions;
                return new Line(first.PointAt(u), second.PointAt(u));
            });
    }

    /// <summary>
    /// Create a hyperbolic paraboloid as lines.
    /// </summary>
    public static IEnumerable<Line> HyperbolicParaboloidAsLines(int divisions, Transform? transform = null)
    {
        transform ??= new();
        Vector3[] vertices =
        {
            transform.OfPoint(new(-.5, -.5, -.5)),
            transform.OfPoint(new(.5, -.5, .5)),
            transform.OfPoint(new(.5, .5, -.5)),
            transform.OfPoint(new(-.5, .5, .5))
        };
        return Enumerable.Empty<Line>()
            .Concat(AsLinesByCurves(
                new Line(vertices[0], vertices[1]),
                new Line(vertices[3], vertices[2]),
                divisions))
            .Concat(AsLinesByCurves(
                new Line(vertices[0], vertices[3]),
                new Line(vertices[1], vertices[2]),
                divisions));
    }

    /// <summary>
    /// Create a conoid as lines.
    /// </summary>
    public static IEnumerable<Curve> ConoidAsLines(int divisions, double rotation, Transform? transform = null)
    {
        transform ??= new();
        Transform rotate = new(0, 0, 0, rotation);
        Curve topA = Polygon.Ngon(divisions)
            .Transformed(new(0, 0, .5))
            .Transformed(transform);
        Curve bottomA = Polygon.Ngon(divisions)
            .Transformed(new(0, 0, -.5))
            .Transformed(rotate)
            .Transformed(transform);
        Curve topB = topA.Transformed(rotate);
        Curve bottomB = bottomA
            .Transformed(rotate.Inverted());
        Curve topC = Polygon.Ngon(100)
            .Transformed(new(0, 0, .5))
            .Transformed(transform);
        Curve bottomC = Polygon.Ngon(100)
            .Transformed(new(0, 0, -.5))
            .Transformed(transform);
        return Enumerable.Empty<Curve>()
            .Append(topC)
            .Append(bottomC)
            .Concat(AsLinesByCurves(topA, bottomA, divisions))
            .Concat(AsLinesByCurves(topB, bottomB, divisions));
    }
}
