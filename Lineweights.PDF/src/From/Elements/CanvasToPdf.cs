using StudioLE.Core.Exceptions;
using Lineweights.Drawings;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to PDF
/// by rendering them and drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class CanvasToPdf : IConverter<Canvas, SKCanvas, SKCanvas>
{
    /// <inheritdoc cref="CanvasToPdf"/>
    public SKCanvas Convert(Canvas canvas, SKCanvas skCanvas)
    {
        ParallelQuery<GeometricElement> geometry = canvas switch
        {
            Sheet sheet => sheet.Render().AsParallel().AsOrdered(),
            View view => view.Render(),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert to pdf.", canvas)
        };

        var converter = new ElementToPdf();
        foreach(GeometricElement element in geometry)
            converter.Convert(element, skCanvas);

        return skCanvas;
    }

    /// <inheritdoc cref="CanvasToPdf"/>
    public SKCanvas Convert(Canvas canvas, SKCanvas skCanvas, Size size)
    {
        skCanvas.Translate(size.Width / 2, size.Height / 2);
        skCanvas.Scale(size.Width / (float)canvas.Width);
        return Convert(canvas, skCanvas);
    }
}
