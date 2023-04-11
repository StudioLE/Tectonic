using Geometrician.Core.Distribution;

namespace Geometrician.Drawings;

/// <summary>
/// A canvas is a 2D box which contains content.
/// It only exists on the XY plane.
/// </summary>
public abstract class Canvas : GeometricElement, IHasBounds
{
    /// <summary>
    /// The 2d untransformed bounds of the canvas.
    /// </summary>
    public BBox3 Bounds { get; set; }

    /// <summary>
    /// The padding between the content of the canvas and the border of the canvas.
    /// </summary>
    /// <remarks>
    /// Follows the
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Box_Model/Introduction_to_the_CSS_box_model">css box model</see>.
    /// </remarks>
    public Spacing Padding { get; set; }

    /// <summary>
    /// The margin between the border of the canvas and other items.
    /// </summary>
    /// <remarks>
    /// Follows the
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Box_Model/Introduction_to_the_CSS_box_model">css box model</see>.
    /// </remarks>
    public Spacing Margin { get; set; }

    /// <summary>
    /// The width of the canvas in the X axis.
    /// </summary>
    public double Width => Bounds.XSize;

    /// <summary>
    /// The height of the canvas in the Y axis.
    /// </summary>
    public double Height => Bounds.YSize;

    /// <summary>
    /// The border of the view.
    /// </summary>
    public Polygon Border => new(PointAt(0, 0), PointAt(1, 0), PointAt(1, 1), PointAt(0, 1));

    /// <summary>
    /// Get a point from the bounding box by supplying normalized parameters from 0 to 1.
    /// </summary>
    public Vector3 PointAt(double u, double v)
    {
        return Bounds.PointAt(u, v, 0);
    }
}
