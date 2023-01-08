using StudioLE.Core.Exceptions;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="ModelCurve"/> or <see cref="Panel"/> to PDF
/// by drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class ElementToPdf : IConverter<Element, SKCanvas>
{
    private readonly SKCanvas _skCanvas;

    /// <inheritdoc cref="ElementToPdf"/>
    public ElementToPdf(SKCanvas skCanvas)
    {
        _skCanvas = skCanvas;
    }

    /// <inheritdoc cref="ElementToPdf"/>
    public SKCanvas Convert(Element element)
    {
        return element switch
        {
            ModelCurve curve => new ModelCurveToPdf(_skCanvas).Convert(curve),
            Panel panel => new PanelToPdf(_skCanvas).Convert(panel),
            // TODO: Implement ModelTextToSvg conversion.
            _ => throw new TypeSwitchException<Element>("Failed to convert element to PDF.", element)
        };
    }
}
