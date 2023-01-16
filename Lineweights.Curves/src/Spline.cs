using Lineweights.Core.Serialization;
using Lineweights.Curves.Interpolation;
using Newtonsoft.Json;
using StudioLE.Core.Exceptions;
using StudioLE.Core.System;

namespace Lineweights.Curves;

/// <summary>
/// A curve through <see cref="KeyVertices"/> that is calculated by its <see cref="Interpolation"/> method.
/// </summary>
public sealed class Spline : Polyline, ICloneable
{
    #region Constants

    private const double Epsilon = Vector3.EPSILON;

    #endregion

    #region Properties

    /// <summary>
    /// The key vertices the curve passes through.
    /// </summary>
    public IReadOnlyCollection<Vector3> KeyVertices { get; private set; }

    /// <summary>
    /// The tangent at the start of the <see cref="Spline"/>.
    /// </summary>
    public Vector3 StartTangent { get; set; }

    /// <summary>
    /// The tangent at the end of the <see cref="Spline"/>.
    /// </summary>
    public Vector3 EndTangent { get; set; }

    /// <summary>
    /// The number of key segments.
    /// </summary>
    public int SegmentCount => KeyVertices.Count - 1;

    /// <summary>
    /// The number of samples to make per key segment when updating the representation.
    /// </summary>
    public int SamplesPerSegment { get; set; } = 10;

    /// <summary>
    /// The number of samples used when updating the representation.
    /// </summary>
    public int SampleCount => SamplesPerSegment * SegmentCount;

    /// <summary>
    /// The interpolation method used to calculate the <see cref="Spline"/>.
    /// </summary>
    [JsonConverter(typeof(AbstractConverter))]
    public IInterpolation Interpolation { get; set; }

    /// <summary>
    /// The frame type to use when calculating transforms along the curve.
    /// </summary>
    public FrameType FrameType { get; set; } = FrameType.RoadLike;

    #endregion

    #region Constructors

    /// <inheritdoc cref="Spline"/>
    public Spline(IReadOnlyCollection<Vector3> keyVertices, IInterpolation? interpolation = null) : base(keyVertices.ToArray())
    {
        KeyVertices = keyVertices;
        Interpolation = interpolation ?? new Linear();
        UpdateRepresentation();
    }

    #endregion

    #region Override methods

    /// <inheritdoc />
    public override BBox3 Bounds()
    {
        // TODO: This is only accurate for linear interpolation.
        return new(KeyVertices);
    }

    /// <inheritdoc />
    public override double Length()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Curve Transformed(Transform transform)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region At methods

    /// <inheritdoc />
    public override Vector3 PointAt(double u)
    {
        Validate.OrThrow(new UParameterConstraint(u, nameof(u)));

        double muFactor = 1d / SegmentCount;
        int segmentIndex = (int)Math.Floor(u * SegmentCount);
        double mu = (u - muFactor * segmentIndex) / muFactor;
        if (mu < 0)
            throw new($"{nameof(Spline)}.{nameof(PointAt)} failed. The mu value ({mu}) was below zero.");
        return PointOnSegmentAt(mu, segmentIndex);
    }

    /// <summary>
    /// Get the point at <paramref name="u"/> on the a segment at <paramref name="segmentIndex"/>.
    /// </summary>
    public Vector3 PointOnSegmentAt(double u, int segmentIndex)
    {
        Validate.OrThrow(new UParameterConstraint(u, nameof(u)));
        Validate.OrThrow(new IndexConstraint(segmentIndex, SegmentCount, "segment", nameof(segmentIndex)));

        int startIndex = segmentIndex;
        if (u.ApproximatelyEquals(0))
            return KeyVertices.ElementAt(startIndex);
        int endIndex = startIndex + 1;
        if (u.ApproximatelyEquals(1))
            return KeyVertices.ElementAt(endIndex);
        int previousIndex = startIndex - 1;
        int nextIndex = startIndex + 2;

        Vector3 startVector = KeyVertices.ElementAt(startIndex);
        Vector3 endVector = KeyVertices.ElementAt(endIndex);

        // TODO: Investigate alternative estimation for beginning and end conditions
        Vector3 previousVector = previousIndex >= 0
            ? KeyVertices.ElementAt(previousIndex)
            : startVector + StartTangent;
        Vector3 nextVector = nextIndex < SegmentCount
            ? KeyVertices.ElementAt(nextIndex)
            : endVector + EndTangent;

        double x = Interpolation.Interpolate(u, startVector.X, endVector.X, previousVector.X, nextVector.X);
        double y = Interpolation.Interpolate(u, startVector.Y, endVector.Y, previousVector.Y, nextVector.Y);
        double z = Interpolation.Interpolate(u, startVector.Z, endVector.Z, previousVector.Z, nextVector.Z);

        return new(x, y, z);
    }

    /// <summary>
    /// Get the tangent to the curve at parameter u.
    /// </summary>
    /// <param name="u">A parameter between 0.0 and 1.0.</param>
    public Vector3 TangentAt(double u)
    {
        Validate.OrThrow(new UParameterConstraint(u, nameof(u)));

        Vector3 previousVertex = u >= Epsilon
            ? PointAt(u - Epsilon)
            : StartTangent + PointAt(0);
        Vector3 nextVertex = u <= 1 - Epsilon
            ? PointAt(u + Epsilon)
            : EndTangent + PointAt(1);

        return (nextVertex - previousVertex).Unitized();
    }

    /// <summary>
    /// Get the normal of the curve at parameter u.
    /// </summary>
    /// <param name="u">A parameter between 0.0 and 1.0.</param>
    public Vector3 NormalAt(double u)
    {
        Validate.OrThrow(new UParameterConstraint(u, nameof(u)));

        Vector3 z = Vector3.ZAxis;

        if (FrameType != FrameType.RoadLike)
        {
            Vector3 vertex = PointAt(u);
            Vector3 previousVertex = u >= Epsilon
                ? PointAt(u - Epsilon)
                : StartTangent + PointAt(0);
            Vector3 nextVertex = u <= 1 - Epsilon
                ? PointAt(u + Epsilon)
                : EndTangent + PointAt(1);
            Plane plane = new(vertex, previousVertex, nextVertex);
            z = plane.Normal;
        }

        return TangentAt(u).Cross(z);
    }

    /// <summary>
    /// Get the transform on the curve at parameter u.
    /// </summary>
    /// <param name="u">The parameter along the curve between 0.0 and 1.0.</param>
    public override Transform TransformAt(double u)
    {
        switch (FrameType)
        {
            case FrameType.Frenet:
                return new(PointAt(u), NormalAt(u), TangentAt(u).Negate());
            case FrameType.RoadLike:
                Vector3 z = TangentAt(u).Negate();
                // If Z is parallel to the Z axis, the other vectors will
                // have zero length. We use the -Y axis in that case.
                Vector3 up = z.IsParallelTo(Vector3.ZAxis) ? Vector3.YAxis.Negate() : Vector3.ZAxis;
                Vector3 x = up.Cross(z);
                return new(PointAt(u), x, z);
            default:
                throw new EnumSwitchException<FrameType>($"{nameof(Spline)}.{nameof(TransformAt)} failed.", FrameType);
        }
    }

    #endregion

    #region Sample methods

    /// <summary>
    /// Update the representation of the spline.
    /// This should be called if <see cref="SamplesPerSegment"/>, <see cref="Interpolation"/> or <see cref="KeyVertices"/> are changed.
    /// </summary>
    public void UpdateRepresentation()
    {
        Vertices = Enumerable
            .Range(0, SampleCount + 1)
            .Select(i => PointAt((double)i / SampleCount))
            .ToArray();

    }

    /// <summary>
    /// Get each of the sampled segments of the spline.
    /// </summary>
    /// <remarks>
    /// TODO: How does this differ from Polyline.Segments()?
    /// </remarks>
    public IEnumerable<Line> SampledSegments()
    {
        return Vertices
            .Take(Vertices.Count - 1)
            .Select((start, i) => new Line(start, Vertices.ElementAt(i + 1)));
    }

    #endregion

    #region Translation methods

    /// <summary>
    /// Offset the <see cref="Spline"/> in the direction determined by <paramref name="flip"/>.
    /// </summary>
    public Spline Offset(double distance, bool flip)
    {
        Spline offsetSpline = this.CloneAs();
        offsetSpline.SamplesPerSegment = 1;
        offsetSpline.KeyVertices = Enumerable.Range(0, SampleCount + 1)
            .Select(i =>
            {
                double u = (double)i / SampleCount;
                // TODO: For FrameType.Frenet the normal switches sides so we need to fix that.
                Vector3 offsetVector = NormalAt(u) * distance;
                if (flip)
                    offsetVector = offsetVector.Negate();
                return PointAt(u) + offsetVector;
            })
            .ToArray();
        offsetSpline.UpdateRepresentation();
        return offsetSpline;
    }

    #endregion

    #region Simplify methods

    /// <summary>
    /// Remove intersections from the spline.
    /// This is often necessary after calling <see cref="Offset(double, bool)"/>. 
    /// </summary>
    public Spline RemoveIntersections()
    {
        if (FrameType != FrameType.RoadLike)
            throw new($"{nameof(Spline)}.{nameof(RemoveIntersections)} failed. Only {nameof(FrameType.RoadLike)} is supported.");

        if (SamplesPerSegment > 1)
            throw new($"{nameof(Spline)}.{nameof(RemoveIntersections)} failed. Sample count must be 1.");

        IReadOnlyCollection<Line> segments = SampledSegments().ToArray();
        IReadOnlyCollection<(int? StartIndex, int? EndIndex, Vector3? Intersection)> intersections = segments
            .SelectMany((a, i) => segments
                .Where((_, ii) => ii < i)
                .Select((b, ii) => Line.Intersects(a.Start, a.End, b.Start, b.End, out Vector3 result)
                    ? ((int?)ii, (int?)i, (Vector3?)result)
                    : (null, null, null)))
            .Where(x => x.Item1 is not null
                        && x.Item2 is not null
                        && x.Item3 is not null)
            .ToArray();

        Spline newSpline = (Spline)Clone();

        // TODO: Move this to a "RemoveSegmentWhere method"
        newSpline.KeyVertices = KeyVertices
            .Select((vertex, i) =>
            {
                if (i == 0 || i == SegmentCount - 1)
                    return vertex;
                // If vertex is the end of the first segment of an intersection
                // Then replace it with the intersection result.
                (int? StartIndex, int? EndIndex, Vector3? Intersection)[] intersectionsOnSegment = intersections
                    .Where(x => i == x.StartIndex + 1)
                    .ToArray();
                if (intersectionsOnSegment.Any())
                    return intersectionsOnSegment.First().Intersection;
                // If vertex is part of a segment to be trimmed
                bool toBeTrimmed = intersections.Any(x => i > x.StartIndex + 1 && i <= x.EndIndex);
                if (toBeTrimmed)
                    return null;

                return vertex;
            })
            .OfType<Vector3>()
            .ToArray();

        return newSpline;
    }

    /// <summary>
    /// Remove segments smaller than <paramref name="tolerance"/>.
    /// </summary>
    public Spline RemoveShortSegments(double tolerance = Epsilon)
    {
        Spline newSpline = (Spline)Clone();
        newSpline.KeyVertices = KeyVertices
            .Where((start, i) =>
            {
                if (i == KeyVertices.Count - 1)
                    return true;
                Vector3 end = KeyVertices.ElementAt(i + 1);
                return start.DistanceTo(end) >= tolerance;
            })
            .ToArray();
        return newSpline;
    }

    #endregion

    #region Clone methods

    /// <inheritdoc />
    public object Clone()
    {
        return new Spline(KeyVertices, Interpolation)
        {
            StartTangent = StartTangent,
            EndTangent = EndTangent,
            FrameType = FrameType,
            SamplesPerSegment = SamplesPerSegment
        };
    }

    #endregion
}
