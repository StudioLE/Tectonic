namespace Geometrician.Core.Elements;

/// <summary>
/// Methods to create <see cref="ModelArrows"/>.
/// </summary>
public static class CreateModelArrows
{
    private const double DefaultArrowHeadFactor = 1;

    /// <summary>
    /// Create <see cref="ModelArrows"/> from vectors and a length.
    /// </summary>
    public static ModelArrows ByVectors(
        Vector3 origin,
        Vector3 direction,
        double length,
        Color? color = null,
        double arrowHeadFactor = DefaultArrowHeadFactor)
    {
        (Vector3, Vector3, double, Color?)[] vectors =
        {
            (Vector3.Origin, direction.Unitized(), arrowHeadFactor, color)
        };
        Transform scale = new();
        scale.Scale(length * (1 / DefaultArrowHeadFactor));
        scale.Move(origin);
        return new(vectors, transform: scale);
    }

    /// <summary>
    /// Create <see cref="ModelArrows"/> from a line.
    /// </summary>
    public static ModelArrows ByLine(
        Line line,
        Color? color = null,
        double arrowHeadFactor = DefaultArrowHeadFactor)
    {
        return ByVectors(line.Start, line.Direction(), line.Length(), color, arrowHeadFactor);
    }

    /// <summary>
    /// Create <see cref="ModelArrows"/> from a <see cref="Transform"/>.
    /// </summary>
    public static ModelArrows ByTransform(
        Transform transform,
        double scaleFactor = 1,
        double arrowHeadFactor = DefaultArrowHeadFactor)
    {
        (Vector3, Vector3, double, Color?)[] vectors =
        {
            (Vector3.Origin, transform.XAxis.Unitized(), arrowHeadFactor, Colors.Red),
            (Vector3.Origin, transform.YAxis.Unitized(), arrowHeadFactor, Colors.Green),
            (Vector3.Origin, transform.ZAxis.Unitized(), arrowHeadFactor, Colors.Blue)
        };
        Vector3 scaleVector = transform.XAxis * scaleFactor * (1 / DefaultArrowHeadFactor)
                              + transform.YAxis * scaleFactor * (1 / DefaultArrowHeadFactor)
                              + transform.ZAxis * scaleFactor * (1 / DefaultArrowHeadFactor);
        Transform scale = new();
        scale.Scale(scaleVector);
        scale.Move(transform.Origin);
        return new(vectors, transform: scale);
    }
}
