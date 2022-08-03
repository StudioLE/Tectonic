namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to help with <see cref="BBox3"/>.
/// </summary>
public static class BBox3Helpers
{
    /// <summary>
    /// Get a <see cref="BBox3"/> containing each of the given <see cref="BBox3"/>.
    /// </summary>
    public static BBox3 Merged(this IEnumerable<BBox3> @this)
    {
        IEnumerable<Vector3> points = @this.SelectMany(x => new[] { x.Min, x.Max });
        return new(points);
    }

    /// <summary>
    /// Determine if the <see cref="BBox3"/> is inverted.
    /// This is often the case if the <see cref="BBox3(Element)"/> constructor is called and can't determine element bounds.
    /// </summary>
    public static bool IsInverted(this BBox3 @this)
    {
        return @this.Min > @this.Max;
    }


    /// <summary>
    /// Determine if the <see cref="BBox3"/> has a near infinite size.
    /// </summary>
    public static bool IsInfinite(this BBox3 @this)
    {
        return @this.Min.X.ApproximatelyEquals(double.MinValue)
               || @this.Min.Y.ApproximatelyEquals(double.MinValue)
               || @this.Min.Z.ApproximatelyEquals(double.MinValue)
               || @this.Max.X.ApproximatelyEquals(double.MaxValue)
               || @this.Max.Y.ApproximatelyEquals(double.MaxValue)
               || @this.Max.Z.ApproximatelyEquals(double.MaxValue);
    }

    /// <summary>
    /// Determine if the <see cref="BBox3"/> is 2 dimensional.
    /// </summary>
    /// <param name="this"></param>
    /// <returns></returns>
    public static bool Is2D(this BBox3 @this)
    {
        return @this.Min.X.ApproximatelyEquals(@this.Max.X)
               || @this.Min.Y.ApproximatelyEquals(@this.Max.Y)
               || @this.Min.Z.ApproximatelyEquals(@this.Max.Z);
    }

    /// <summary>
    /// Create an <see cref="Extrude"/> of the <see cref="BBox3"/>.
    /// </summary>
    public static Extrude ToExtrude(this BBox3 @this)
    {
        var rectangle = new Polygon(
            @this.PointAt(0, 0, 0),
            @this.PointAt(0, 1, 0),
            @this.PointAt(1, 1, 0),
            @this.PointAt(1, 0, 0)
        );
        return new(rectangle, @this.Max.Z - @this.Min.Z, Vector3.ZAxis, false);
    }

    /// <summary>
    /// Create a <see cref="Mass"/> of the <see cref="BBox3"/>.
    /// </summary>
    public static Mass ToMass(this BBox3 @this, Transform? context = null, Material? material = null)
    {
        var rectangle = new Polygon(
            @this.PointAt(0, 0, 0),
            @this.PointAt(0, 1, 0),
            @this.PointAt(1, 1, 0),
            @this.PointAt(1, 0, 0)
        );
        return new(rectangle, @this.Max.Z - @this.Min.Z, material, context);
    }
}
