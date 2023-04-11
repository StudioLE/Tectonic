namespace Geometrician.PDF.From.Geometry;

/// <summary>
/// Convert a <see cref="Vector3"/> to a <see cref="SKPoint"/>.
/// </summary>
internal sealed class Vector3ToPdf : IConverter<Vector3, SKPoint>
{
    /// <inheritdoc cref="Vector3ToPdf"/>
    public SKPoint Convert(Vector3 point)
    {
        return new((float)point.X, (float)point.Y * -1);
    }
}
