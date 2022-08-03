namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to create <see cref="Arc"/>.
/// </summary>
public static class CreateArc
{
    /// <summary>
    /// Create an arc from three points.
    /// </summary>
    public static Arc ByThreePoints(Vector3 start, Vector3 end, Vector3 point)
    {
        Circle circle = CreateCircle.ByThreePoints(start, end, point);
        double startAngle = Vector3.XAxis.PlaneAngleTo(start - circle.Center);
        double endAngle = Vector3.XAxis.PlaneAngleTo(end - circle.Center);
        double pointAngle = Vector3.XAxis.PlaneAngleTo(point - circle.Center);
        if (startAngle > endAngle)
            (startAngle, endAngle) = (endAngle, startAngle);
        if (pointAngle < startAngle || pointAngle > endAngle)
            endAngle -= 360;
        return new(circle.Center, circle.Radius, startAngle, endAngle);
    }

    /// <summary>
    /// Create an arc from a polyline.
    /// </summary>
    public static Arc ByPolyline(Polyline polyline)
    {
        Vector3 pointOnArc = polyline
            .Start
            .Average(polyline.End)
            .ClosestPointOn(polyline);
        return ByThreePoints(polyline.Start, polyline.End, pointOnArc);
        
    }
}
