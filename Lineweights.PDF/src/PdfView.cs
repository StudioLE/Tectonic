using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;

namespace Lineweights.PDF;

/// <summary>
/// A <see cref="PdfDocument"/> representing a <see cref="View"/>.
/// </summary>
public sealed class PdfView : PdfDocument
{
    private readonly View _view;

    /// <inheritdoc cref="PdfView"/>
    public PdfView(View view) : base(view)
    {
        _view = view;
    }

    /// <inheritdoc />
    protected override void ComposeLayers(LayersDescriptor layers)
    {
        layers
            .PrimaryLayer()
            .Canvas((skCanvas, size) => new CanvasToSkCanvas(skCanvas, size).Convert(_view));
    }
}
