using Geometrician.PDF.From.Geometry;

namespace Geometrician.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="ModelCurve"/> to PDF
/// by drawing the 2d curve on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class ModelCurveToSkCanvas : IConverter<ModelCurve, SKCanvas>
{
    private readonly SKCanvas _skCanvas;

    /// <inheritdoc cref="ModelCurveToSkCanvas"/>
    public ModelCurveToSkCanvas(SKCanvas skCanvas)
    {
        _skCanvas = skCanvas;
    }

    /// <inheritdoc cref="ModelCurveToSkCanvas"/>
    public SKCanvas Convert(ModelCurve element)
    {
        Curve curve = element.Curve.Transformed(element.Transform);
        SKPath path = new CurveToPdf().Convert(curve);
        SKColor color = new ColorToPdf().Convert(element.Material.Color);
        SKPaint paint = new()
        {
            StrokeWidth = .001f,
            IsStroke = true,
            Color = color
        };
        _skCanvas.DrawPath(path, paint);
        return _skCanvas;
    }
}
