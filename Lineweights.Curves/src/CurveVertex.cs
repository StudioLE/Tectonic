namespace Lineweights.Curves;

/// <summary>
/// A vertex of a <see cref="Curve"/>.
/// </summary>
internal sealed class CurveVertex
{
    /// <summary>
    /// The index of the segment.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The total count of segments.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// The current vertex.
    /// </summary>
    public Vector3 Current { get; }

    /// <summary>
    /// The previous vertex.
    /// Or <see langword="null"/> if this is the first vertex.
    /// </summary>
    public Vector3? Previous { get; }

    /// <summary>
    /// The next vertex.
    /// Or <see langword="null"/> if this is the last vertex.
    /// </summary>
    public Vector3? Next { get; }

    /// <summary>
    /// The vector from <see cref="Previous"/> to <see cref="Current"/>.
    /// Or <see langword="null"/> if this is the first vertex.
    /// </summary>
    public Vector3? VectorFromPrevious => IsFirst
        ? null
        : Current - Previous;

    /// <summary>
    /// The vector from <see cref="Current"/> to <see cref="Next"/>.
    /// Or <see langword="null"/> if this is the last vertex.
    /// </summary>
    public Vector3? VectorToNext => IsLast
        ? null
        : Next - Current;

    /// <summary>
    /// Determine if this is the first vertex.
    /// </summary>
    public bool IsFirst => Index == 0;

    /// <summary>
    /// Determine if this is the last vertex.
    /// </summary>
    public bool IsLast => Index == TotalCount - 1;

    /// <inheritdoc cref="CurveVertex"/>
    public CurveVertex(Polyline polyline, int index)
    {
        Validate.OrThrow(new IndexConstraint(index, polyline.Vertices.Count, "vertex", nameof(index)));
        Index = index;
        TotalCount = polyline.Vertices.Count;
        Current = polyline.Vertices[index];
        Previous = IsFirst
            ? null
            : polyline.Vertices[index - 1];
        Next = IsLast
            ? null
            : polyline.Vertices[index + 1];
    }
}
