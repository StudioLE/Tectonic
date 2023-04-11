using StudioLE.Core.Exceptions;

namespace Geometrician.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="ModelCurve"/> or <see cref="Panel"/> to PDF
/// by drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class ElementToSkCanvas : IConverter<Element, SKCanvas>
{
    private readonly SKCanvas _skCanvas;

    /// <inheritdoc cref="ElementToSkCanvas"/>
    public ElementToSkCanvas(SKCanvas skCanvas)
    {
        _skCanvas = skCanvas;
    }

    /// <inheritdoc cref="ElementToSkCanvas"/>
    public SKCanvas Convert(Element element)
    {
        return element switch
        {
            ModelCurve curve => new ModelCurveToSkCanvas(_skCanvas).Convert(curve),
            Panel panel => new PanelToSkCanvas(_skCanvas).Convert(panel),
            // TODO: Implement ModelTextToSvg conversion.
            _ => throw new TypeSwitchException<Element>("Failed to convert element to PDF.", element)
        };
    }
}
