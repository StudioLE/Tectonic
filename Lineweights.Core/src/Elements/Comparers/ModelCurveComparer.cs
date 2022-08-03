using Lineweights.Core.Geometry.Comparers;

namespace Lineweights.Core.Elements.Comparers;

/// <summary>
/// Compare the equality of <see cref="ModelCurve"/> by their properties.
/// </summary>
public sealed class ModelCurveComparer : IEqualityComparer<ModelCurve>
{
    /// <inheritdoc cref="ModelCurveComparer"/>
    public bool Equals(ModelCurve first, ModelCurve second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first.GetType() != second.GetType())
            return false;

        if (first.Curve.GetType() != second.Curve.GetType())
            return false;

        // TODO: Handle comparison of Arcs etc.
        bool curvesMatch = first.Curve switch
        {
            Line => new LineComparer().Equals((Line)first.Curve, (Line)second.Curve),
            Polygon => new PolygonComparer().Equals((Polygon)first.Curve, (Polygon)second.Curve),
            Polyline => new PolylineComparer().Equals((Polyline)first.Curve, (Polyline)second.Curve),
            _ => false
        };

        return curvesMatch
               && new TransformComparer().Equals(first.Transform, second.Transform)
               && new MaterialComparer().Equals(first.Material, second.Material);
    }

    /// <inheritdoc cref="ModelCurveComparer"/>
    public int GetHashCode(ModelCurve obj)
    {
        HashCode hashCode = new();
        // TODO: Handle comparison of Arcs etc.
        switch (obj.Curve)
        {
            case Line line:
                hashCode.Add(line, new LineComparer());
                break;
            case Polygon polygon:
                hashCode.Add(polygon, new PolygonComparer());
                break;
            case Polyline polyline:
                hashCode.Add(polyline, new PolylineComparer());
                break;
        }
        hashCode.Add(obj.Transform, new TransformComparer());
        hashCode.Add(obj.Material, new MaterialComparer());
        return hashCode.ToHashCode();
    }
}
