using Geometrician.PDF.From.Geometry;

namespace Geometrician.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="Panel"/> to PDF
/// by drawing the 2d filled polygon on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class PanelToSkCanvas : IConverter<Panel, SKCanvas>
{
    private readonly SKCanvas _skCanvas;

    /// <inheritdoc cref="PanelToSkCanvas"/>
    public PanelToSkCanvas(SKCanvas skCanvas)
    {
        _skCanvas = skCanvas;
    }

    /// <inheritdoc cref="PanelToSkCanvas"/>
    public SKCanvas Convert(Panel element)
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
        _skCanvas.DrawPath(path, fill);
        _skCanvas.DrawPath(path, stroke);
        return _skCanvas;
    }
}
