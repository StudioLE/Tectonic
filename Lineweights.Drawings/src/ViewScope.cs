namespace Lineweights.Drawings;

/// <summary>
/// The scope which is captured by a a <see cref="View"/>.
/// </summary>
public sealed class ViewScope : Element
{
    /// <summary>
    /// The center of the view scope plane.
    /// </summary>
    public Vector3 Origin { get; set; }

    /// <summary>
    /// The direction to the right of the view scope.
    /// </summary>
    public Vector3 RightDirection { get; set; }

    /// <summary>
    /// The direction to the top of the view scope.
    /// </summary>
    public Vector3 UpDirection { get; set; }

    /// <summary>
    /// The direction away from the the viewer.
    /// </summary>
    public Vector3 FacingDirection { get; set; }

    /// <summary>
    /// The width of the view scope.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// The height of the view scope.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// The depth of the view scope.
    /// </summary>
    public double Depth { get; set; }

    /// <summary>
    /// The elements in the view scope.
    /// </summary>
    public IList<Element> Elements { get; set; } = Array.Empty<Element>();

    /// <summary>
    /// The plane of the view scope.
    /// </summary>
    public Plane Plane => new(Origin, FacingDirection);

    /// <summary>
    /// The orientation of the view scope.
    /// </summary>
    public Transform Orientation => new(Vector3.Origin, RightDirection, UpDirection, FacingDirection.Negate());

    /// <summary>
    /// Get the outline of the view scope plane.
    /// </summary>
    public Polygon Border
    {
        get
        {
            Vector3 bottomLeft = Origin
                                 - RightDirection * Width * 0.5
                                 - UpDirection * Height * 0.5;
            Vector3 bottomRight = Origin
                                  + RightDirection * Width * 0.5
                                  - UpDirection * Height * 0.5;
            Vector3 topRight = Origin
                               + RightDirection * Width * 0.5
                               + UpDirection * Height * 0.5;
            Vector3 topLeft = Origin
                              - RightDirection * Width * 0.5
                              + UpDirection * Height * 0.5;
            return new(bottomLeft, bottomRight, topRight, topLeft);
        }
    }

    internal ViewScope()
    {
    }

    /// <summary>
    /// Get the box of the view scope plane.
    /// </summary>
    public Box ToBox()
    {
        Transform transform = new(Vector3.Origin, RightDirection, UpDirection, FacingDirection);
        // TODO: Use actual dimensions rather than scaling...
        Vector3 scale = RightDirection * Width
                        + UpDirection * Height
                        + FacingDirection * Depth;
        transform.Scale(scale);
        Vector3 center = Origin + FacingDirection * Depth * 0.5;
        transform.Move(center);
        return new(CreateBBox3.ByLengths(1, 1, 1))
        {
            Transform = transform
        };
    }
}
