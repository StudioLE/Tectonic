using Lineweights.PDF.From.Geometry;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="ModelCurve"/> to PDF
/// by drawing the 2d curve on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class ModelCurveToPdf : IConverter<ModelCurve, SKCanvas, SKCanvas>
{
    /// <inheritdoc cref="ModelCurveToPdf"/>
    public SKCanvas Convert(ModelCurve element, SKCanvas canvas)
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
        canvas.DrawPath(path, paint);
        return canvas;
    }
}
