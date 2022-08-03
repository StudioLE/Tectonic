using Lineweights.PDF.From.Geometry;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="Panel"/> to PDF
/// by drawing the 2d filled polygon on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class PanelToPdf : IConverter<Panel, SKCanvas, SKCanvas>
{
    /// <inheritdoc cref="PanelToPdf" />
    public SKCanvas Convert(Panel element, SKCanvas canvas)
    {
        Polygon polygon = element.Perimeter.TransformedPolygon(element.Transform);
        SKColor color = new ColorToPdf().Convert(element.Material.Color);
        SKPath path = new CurveToPdf().Convert(polygon);
        SKPaint fill = new()
        {
            IsStroke = false,
            Color = color
        };
        SKPaint stroke = new()
        {
            StrokeWidth = .0002f,
            IsStroke = true,
            Color = SKColors.Black
        };
        canvas.DrawPath(path, fill);
        canvas.DrawPath(path, stroke);
        return canvas;
    }
}
