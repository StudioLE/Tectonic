using Geometrician.Drawings;
using StudioLE.Core.Exceptions;

namespace Geometrician.PDF.From.Elements;

/// <summary>
/// Convert either a <see cref="Sheet"/> or <see cref="View"/> to PDF
/// by rendering them and drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class CanvasToSkCanvas : IConverter<Canvas, SKCanvas>
{
    private readonly SKCanvas _skCanvas;
    private readonly Size? _size;

    /// <inheritdoc cref="CanvasToSkCanvas"/>
    public CanvasToSkCanvas(SKCanvas skCanvas, Size? size = null)
    {
        _skCanvas = skCanvas;
        _size = size;
    }

    /// <inheritdoc cref="CanvasToSkCanvas"/>
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

        ElementToSkCanvas converter = new(_skCanvas);
        foreach (GeometricElement element in geometry)
            converter.Convert(element);

        return _skCanvas;
    }
}
