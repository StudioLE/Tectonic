using Lineweights.Drawings;
using StudioLE.Core.Exceptions;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to PDF
/// by rendering them and drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class CanvasToPdf : IConverter<Canvas, SKCanvas>
{
    private readonly SKCanvas _skCanvas;
    private readonly Size? _size;

    /// <inheritdoc cref="CanvasToPdf"/>
    public CanvasToPdf(SKCanvas skCanvas, Size? size = null)
    {
        _skCanvas = skCanvas;
        _size = size;
    }

    /// <inheritdoc cref="CanvasToPdf"/>
    public SKCanvas Convert(Canvas canvas)
    {
        if (_size is Size size)
        {
            _skCanvas.Translate(size.Width / 2, size.Height / 2);
            _skCanvas.Scale(size.Width / (float)canvas.Width);
        }

        ParallelQuery<GeometricElement> geometry = canvas switch
        {
            Sheet sheet => sheet.Render().AsParallel().AsOrdered(),
            View view => view.Render(),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert to pdf.", canvas)
        };

        var converter = new ElementToPdf(_skCanvas);
        foreach (GeometricElement element in geometry)
            converter.Convert(element);

        return _skCanvas;
    }
}
