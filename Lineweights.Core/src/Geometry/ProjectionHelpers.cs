using Ardalis.Result;

namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to project geometry.
/// </summary>
public static class ProjectionHelpers
{
    /// <summary>
    /// Try and project <paramref name="curve"/> onto <paramref name="plane"/>.
    /// This method tries Line, Polygon, and then Polyline projection sequentially
    /// to find the best possible projection.
    /// </summary>
    public static Result<Curve> TryProject(this Curve curve, Plane plane)
    {
        List<IResult> results = new();
        if (curve is Line line)
            results.Add(line.TryProject(plane));
        if (curve is Polygon polygon)
            results.Add(polygon.TryProject(plane));
        if (curve is Polyline polyline)
            results.Add(polyline.TryProject(plane));

        IResult? result = results.FirstOrDefault(x => x.Status == ResultStatus.Ok);

        return result is not null
            ? (Curve)result.GetValue()
            : Result<Curve>.Error("Failed to find a projection.");
    }

    /// <summary>
    /// Try and project <paramref name="this"/> onto <paramref name="plane"/>.
    /// </summary>
    public static Result<Line> TryProject(this Line @this, Plane plane)
    {
        try
        {
            Vector3 start = @this.Start.Project(plane);
            Vector3 end = @this.End.Project(plane);
            if (start.IsAlmostEqualTo(end))
                return Result<Line>.Error("Line is perpendicular to view.");
            return new Line(start, end);
        }
        catch (Exception e)
        {
            return Result<Line>.Error(e.Message);
        }
    }

    /// <summary>
    /// Try and project <paramref name="this"/> onto <paramref name="plane"/>.
    /// </summary>
    public static Result<Polyline> TryProject(this Polyline @this, Plane plane)
    {
        try
        {
            return @this.Project(plane);
        }
        catch (Exception e)
        {
            return Result<Polyline>.Error(e.Message);
        }
    }

    /// <summary>
    /// Try and project <paramref name="this"/> onto <paramref name="plane"/>.
    /// </summary>
    public static Result<Polygon> TryProject(this Polygon @this, Plane plane)
    {
        try
        {
            Vector3[] projected = @this.Vertices.Project(plane).ToArray();
            if (projected.Length < 3)
                return Result<Polygon>.Error("At least 3 vertices are required.");
            if (projected.AreCollinear())
                return Result<Polygon>.Error("Vertices are collinear.");
            return new Polygon(projected);
        }
        catch (Exception e)
        {
            return Result<Polygon>.Error(e.Message);
        }
    }

    /// <summary>
    /// Try and project <paramref name="vertices"/> onto <paramref name="plane"/>.
    /// </summary>
    public static IEnumerable<Vector3> Project(this IEnumerable<Vector3> vertices, Plane plane)
    {
        return vertices
            .Select(x => x.Project(plane))
            .ToArray()
            .RemoveSequentialDuplicates();
    }

    /// <see href="https://github.com/hypar-io/Elements/blob/42d21aca5200788561d04c99daf3c6e2aa924b76/Elements/src/Geometry/Vector3.cs#L876-L900"/>
    private static IEnumerable<Vector3> RemoveSequentialDuplicates(
        this IReadOnlyCollection<Vector3> vertices,
        bool wrap = false,
        double tolerance = Vector3.EPSILON)
    {
        Vector3 previous = new();
        return vertices
            .Select<Vector3, Vector3?> ((current, i) =>
            {
                if (i == 0)
                {
                    previous = current;
                    return current;
                }

                if (current.IsAlmostEqualTo(previous, tolerance))
                    return null;

                // if we wrap, and we're at the last vertex, also check for a zero-length segment between first and last.
                if (wrap && i == vertices.Count - 1 && current.IsAlmostEqualTo(vertices.First(), tolerance))
                    return null;

                previous = current;
                return current;
            })
            .OfType<Vector3>();
    }
}
