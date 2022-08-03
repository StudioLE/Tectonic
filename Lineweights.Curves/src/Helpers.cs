using static MoreLinq.Extensions.BatchExtension;

namespace Lineweights.Curves;

/// <summary>
/// Methods to help with <see cref="Curve"/>.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Create arcs from segments of a polyline where the distance between arc centers is less than <paramref name="threshold"/>.
    /// </summary>
    public static IReadOnlyCollection<Curve> CombineSegmentsToArcs(Polyline polyline, double threshold)
    {
        List<Vector3> groupCenters = new();
        List<List<CurveSegment>> groups = new() { new() };
        foreach (CurveSegment segment in polyline.CurveSegments())
        {
            if (segment.IsLast)
            {
                groups.Last().Add(segment);
                continue;
            }

            Vector3 center = CreateCircle
                .ByThreePoints(segment.StartVertex, segment.EndVertex, (Vector3)segment.VectorToNext!)
                .Center;

            Vector3? averageCenter = groupCenters.Any()
                ? groupCenters.Average()
                : null;

            double distance = averageCenter is null
                ? 0
                : ((Vector3)averageCenter!).DistanceTo(center);
            bool isWithinThreshold = distance <= threshold;
            if ((segment.IsFirst || segment.IsSameDirectionAsPrevious) && isWithinThreshold)
            {
                groupCenters.Add(center);
                groups.Last().Add(segment);
                continue;
            }
            groups.Add(new() { segment });
            groupCenters.Clear();
        }

        return groups
            .Select<List<CurveSegment>, Curve>(group => group.Count switch
            {
                1 => group.First().Line,
                _ => CreateArc.ByPolyline(new(group.Select(x => x.StartVertex).Append(group.Last().EndVertex).ToArray()))
            })
            .ToArray();
    }

    /// <inheritdoc cref="CombineSegmentsToArcs(Polyline, double)"/>
    public static IReadOnlyCollection<Curve> CombineSegmentsToArcs(Spline spline, double threshold, bool preserveKeyVertices)
    {
        if (!preserveKeyVertices)
            return CombineSegmentsToArcs(spline, threshold);

        IEnumerable<Polyline> polylines = spline
            .Segments()
            .Batch(spline.SamplesPerSegment)
            .Select(lines => new Polyline(lines
                .SelectMany(line => new [] { line.Start, line.End })
                .ToArray()));

        return polylines
            .SelectMany(polyline => CombineSegmentsToArcs(polyline, threshold))
            .ToArray();
    }
}
