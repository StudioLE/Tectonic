using StudioLE.Core.Exceptions;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="ModelCurve"/> or <see cref="Panel"/> to PDF
/// by drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class ElementToPdf : IConverter<Element, SKCanvas, SKCanvas>
{
    /// <inheritdoc cref="ElementToPdf"/>
    public SKCanvas Convert(Element element, SKCanvas canvas)
    {
        return element switch
        {
            ModelCurve curve => new ModelCurveToPdf().Convert(curve, canvas),
            Panel panel => new PanelToPdf().Convert(panel, canvas),
            // TODO: Implement ModelTextToSvg conversion.
            _ => throw new TypeSwitchException<Element>("Failed to convert element to PDF.", element)
        };
    }
}
