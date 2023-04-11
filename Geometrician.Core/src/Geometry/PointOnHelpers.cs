namespace Geometrician.Core.Geometry;

/// <summary>
/// Methods to find points on geometry.
/// </summary>
public static class PointOnHelpers
{
    /// <inheritdoc cref="Vector3.ClosestPointOn(Line)"/>
    public static Vector3 ClosestPointOn(this Vector3 @this, Polyline polyline)
    {
        return polyline
            .Segments()
            .Select(@this.ClosestPointOn)
            .OrderBy(@this.DistanceTo)
            .First();
    }

    /// <inheritdoc cref="Vector3.ClosestPointOn(Line)"/>
    /// <remarks>
    /// WARNING: This is only an approximation.
    /// </remarks>
    public static Vector3 ClosestPointOn(this Vector3 @this, Curve curve, int segmentCount = 100)
    {
        return @this.ClosestPointOn(curve.ToPolyline(segmentCount));
    }

    /// <summary>
    /// Determine if <paramref name="this"/> is a point on the <paramref name="polyline"/>.
    /// </summary>
    public static bool IsPointOn(this Vector3 @this, Polyline polyline, double threshold = Vector3.EPSILON)
    {
        return @this.DistanceTo(polyline) < threshold;
    }

    /// <summary>
    /// Determine if <paramref name="this"/> is a point on the <paramref name="curve"/>.
    /// </summary>
    /// <remarks>
    /// WARNING: This is only an approximation.
    /// </remarks>
    public static bool IsPointOn(this Vector3 @this, Curve curve, int segmentCount = 100, double threshold = Vector3.EPSILON)
    {
        return @this.IsPointOn(curve.ToPolyline(segmentCount), threshold);
    }
}
