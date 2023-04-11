using StudioLE.Core.Results;

namespace Geometrician.Core.Geometry;

/// <summary>
/// Methods to create <see cref="BBox3"/>.
/// </summary>
public static class CreateBBox3
{
    /// <summary>
    /// Create a BBox3 from a width, length, and height.
    /// </summary>
    public static BBox3 ByLengths(double width, double length, double height)
    {
        Vector3 min = new(width * 0.5 * -1, length * 0.5 * -1, height * 0.5 * -1);
        Vector3 max = new(width * 0.5, length * 0.5, height * 0.5);
        return new(min, max);
    }

    /// <summary>
    /// Create a 2D BBox3 from a width and height.
    /// </summary>
    public static BBox3 ByLengths2d(double width, double height)
    {
        return ByLengths(width, height, 0);
    }

    /// <summary>
    /// Create a BBox3 from a width, length, and height.
    /// </summary>
    public static BBox3 ByElements(IEnumerable<Element> elements)
    {
        return elements
            .SelectMany(element =>
            {
                IResult<BBox3> result = element.TryGetTransformedBounds();
                return result is Success<BBox3> success
                    ? new[] { success.Value }
                    : Enumerable.Empty<BBox3>();
            })
            .Merged();
    }
}
