namespace Lineweights.Core.Geometry.Comparers;

/// <summary>
/// Compare the equality of <see cref="Polyline"/> by their properties.
/// </summary>
public sealed class PolylineComparer : IEqualityComparer<Polyline>
{
    /// <inheritdoc cref="Polyline"/>
    public bool Equals(Polyline first, Polyline second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first.GetType() != second.GetType())
            return false;

        return first.Vertices.SequenceEqual(second.Vertices, new Vector3Comparer());
    }

    /// <inheritdoc cref="Polyline"/>
    public int GetHashCode(Polyline obj)
    {
        Vector3Comparer comparer = new();
        HashCode hashCode = new();
        foreach (Vector3 vertex in obj.Vertices)
            hashCode.Add(comparer.GetHashCode(vertex));
        return hashCode.ToHashCode();
    }
}

/// <summary>
/// Compare the equality of <see cref="Polygon"/> by their properties.
/// </summary>
public sealed class PolygonComparer : IEqualityComparer<Polygon>
{
    /// <inheritdoc cref="Polygon"/>
    public bool Equals(Polygon first, Polygon second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first.GetType() != second.GetType())
            return false;

        return first.Vertices.SequenceEqual(second.Vertices, new Vector3Comparer());
    }

    /// <inheritdoc cref="Polygon"/>
    public int GetHashCode(Polygon obj)
    {
        Vector3Comparer comparer = new();
        HashCode hashCode = new();
        foreach (Vector3 vertex in obj.Vertices)
            hashCode.Add(comparer.GetHashCode(vertex));
        return hashCode.ToHashCode();
    }
}
