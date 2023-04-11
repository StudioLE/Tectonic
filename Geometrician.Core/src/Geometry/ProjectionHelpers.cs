using StudioLE.Core.Results;

namespace Geometrician.Core.Geometry;

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
    public static IResult<Curve> TryProject(this Curve curve, Plane plane)
    {
        if (curve is Line line)
        {
            IResult<Line> result = line.TryProject(plane);
            if (result is Success<Line> success)
                return new Success<Curve>(success.Value);
        }
        if (curve is Polygon polygon)
        {
            IResult<Polygon> result = polygon.TryProject(plane);
            if (result is Success<Polygon> success)
                return new Success<Curve>(success.Value);
        }
        if (curve is Polyline polyline)
        {
            IResult<Polyline> result = polyline.TryProject(plane);
            if (result is Success<Polyline> success)
                return new Success<Curve>(success.Value);
        }
        return new Failure<Curve>("Failed to find a projection.");
    }

    /// <summary>
    /// Try and project <paramref name="this"/> onto <paramref name="plane"/>.
    /// </summary>
    public static IResult<Line> TryProject(this Line @this, Plane plane)
    {
        try
        {
            Vector3 start = @this.Start.Project(plane);
            Vector3 end = @this.End.Project(plane);
            if (start.IsAlmostEqualTo(end))
                return new Failure<Line>("Line is perpendicular to view.");
            return new Success<Line>(new(start, end));
        }
        catch (Exception e)
        {
            return new Failure<Line>(e);
        }
    }

    /// <summary>
    /// Try and project <paramref name="this"/> onto <paramref name="plane"/>.
    /// </summary>
    public static IResult<Polyline> TryProject(this Polyline @this, Plane plane)
    {
        try
        {
            return new Success<Polyline>(@this.Project(plane));
        }
        catch (Exception e)
        {
            return new Failure<Polyline>(e);
        }
    }

    /// <summary>
    /// Try and project <paramref name="this"/> onto <paramref name="plane"/>.
    /// </summary>
    public static IResult<Polygon> TryProject(this Polygon @this, Plane plane)
    {
        try
        {
            Vector3[] projected = @this.Vertices.Project(plane).ToArray();
            if (projected.Length < 3)
                return new Failure<Polygon>("At least 3 vertices are required.");
            if (projected.AreCollinear())
                return new Failure<Polygon>("Vertices are collinear.");
            return new Success<Polygon>(new(projected));
        }
        catch (Exception e)
        {
            return new Failure<Polygon>(e);
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
            .Select<Vector3, Vector3?>((current, i) =>
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
