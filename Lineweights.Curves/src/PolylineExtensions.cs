namespace Lineweights.Curves;

/// <summary>
/// Methods to extend <see cref="Polyline"/>.
/// </summary>
public static class PolylineExtensions
{
    /// <summary>
    /// Get all the <see cref="CurveVertex"/> of <paramref name="this"/>.
    /// </summary>
    internal static IReadOnlyList<CurveVertex> CurveVertices(this Polyline @this)
    {
        return Enumerable
            .Range(0, @this.Vertices.Count)
            .Select(i => new CurveVertex(@this, i))
            .ToArray();
    }

    /// <summary>
    /// Get all the <see cref="CurveSegment"/> of <paramref name="this"/>.
    /// </summary>
    internal static IReadOnlyList<CurveSegment> CurveSegments(this Polyline @this)
    {
        return Enumerable
            .Range(0, @this.Vertices.Count - 1)
            .Select(i => new CurveSegment(@this, i))
            .ToArray();
    }
}
