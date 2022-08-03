namespace Lineweights.Curves;

/// <summary>
/// A segment of a [](xref:Elements.Geometry.Curve).
/// </summary>
internal sealed class CurveSegment
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
    /// The vertex at the start of the segment.
    /// </summary>
    public Vector3 StartVertex { get; }

    /// <summary>
    /// The vertex at the end of the segment.
    /// </summary>
    public Vector3 EndVertex { get; }

    /// <summary>
    /// The vertex at the start of the previous segment.
    /// Or <see langword="null"/> if this is the first segment.
    /// .</summary>
    public Vector3? PreviousVertex { get; }

    /// <summary>
    /// The vertex at the end of the next segment.
    /// Or <see langword="null"/> if this is the last segment.
    /// .</summary>
    public Vector3? NextVertex { get; }

    /// <summary>
    /// The segment.
    /// </summary>
    public Line Line { get; }

    /// <summary>
    /// The vector from <see cref="StartVertex"/> to <see cref="EndVertex"/>.
    /// </summary>
    public Vector3 VectorToEnd => EndVertex - StartVertex;

    /// <summary>
    /// The vector from <see cref="PreviousVertex"/> to <see cref="StartVertex"/>.
    /// Or <see langword="null"/> if this is the first segment.
    /// </summary>
    public Vector3? VectorFromPrevious => IsFirst ? null : StartVertex - PreviousVertex;

    /// <summary>
    /// The vector from <see cref="EndVertex"/> to <see cref="NextVertex"/>.
    /// Or <see langword="null"/> if this is the last segment.
    /// </summary>
    public Vector3? VectorToNext => IsLast ? null : NextVertex - EndVertex;

    /// <summary>
    /// Determine if this is the first segment.
    /// </summary>
    public bool IsFirst => Index == 0;

    /// <summary>
    /// Determine if this is the last segment.
    /// </summary>
    public bool IsLast => Index == TotalCount - 1;

    /// <summary>
    /// The <see cref="RotationHelpers.SignedPlaneAngleTo(Vector3, Vector3)"/> from the previous segment.
    /// </summary>
    public double? SignedAngleFromPrevious => IsFirst ? null : ((Vector3)VectorFromPrevious!).SignedPlaneAngleTo(VectorToEnd);

    /// <summary>
    /// The <see cref="RotationHelpers.SignedPlaneAngleTo(Vector3, Vector3)"/> to the next segment.
    /// </summary>
    public double? SignedAngleToNext => IsLast ? null : VectorToEnd.SignedPlaneAngleTo((Vector3)VectorToNext!);

    /// <summary>
    /// Determine if <see cref="SignedAngleFromPrevious"/> and <see cref="SignedAngleToNext"/> are in the same direction.
    /// </summary>
    public bool IsSameDirectionAsPrevious => (SignedAngleFromPrevious is not null && SignedAngleToNext is not null)
                                             && (SignedAngleFromPrevious <= 0 && SignedAngleToNext <= 0
                                                 || SignedAngleFromPrevious >= 0 && SignedAngleToNext >= 0);
    /// <inheritdoc cref="CurveSegment"/>
    public CurveSegment(Polyline polyline, int index)
    {
        Validate.OrThrow(new IndexConstraint(index, polyline.Vertices.Count, "segment", nameof(index)));
        Index = index;

        TotalCount = polyline.Vertices.Count - 1;
        StartVertex = polyline.Vertices[index];
        EndVertex = polyline.Vertices[index + 1];
        PreviousVertex = IsFirst
            ? null
            : polyline.Vertices[index - 1];
        NextVertex = IsLast
            ? null
            : polyline.Vertices[index + 2];
        Line = new(StartVertex, EndVertex);
    }
}
