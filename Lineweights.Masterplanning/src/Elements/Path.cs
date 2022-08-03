using Lineweights.Curves;

namespace Lineweights.Masterplanning.Elements;

/// <summary>
/// A path such as a road, footway, track,  pavement, or sidewalk.
/// </summary>
public sealed class Path : Floor
{
    /// <summary>
    /// The centerline of the path as a spline.
    /// TODO: shouldn't this be a curve?
    /// </summary>
    public Spline CenterLine { get; set; }

    /// <summary>
    /// The width of the path.
    /// </summary>
    public double Width { get; set; }

    /// <inheritdoc cref="Path"/>
    public Path(
        Spline centerLine,
        double width,
        double thickness,
        Transform transform = null!,
        Material material = null!,
        string name = null!
        ) : base(
        CreateProfile(centerLine, width),
        thickness,
        transform,
        material,
        name: name)
    {
        CenterLine = centerLine;
        Width = width;
    }

    /// <summary>
    /// Create the profile of the path by offsetting from the <see cref="CenterLine"/>.
    /// </summary>
    private static Profile CreateProfile(Spline centerSpline, double width)
    {
        Spline nearSpline = centerSpline.Offset(width / 2, true);
        Spline farSpline = centerSpline.Offset(width / 2, false);
        Vector3[] vertices =
            Enumerable.Empty<Vector3>()
                .Concat(nearSpline.Vertices)
                .Concat(farSpline.Vertices.Reverse())
                .ToArray();
        return new(new Polygon(vertices));
    }
}
