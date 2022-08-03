using StudioLE.Core.Exceptions;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert an <see cref="Element"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class ElementToSvg : IConverter<Element, SvgElement>
{
    /// <inheritdoc cref="ElementToSvg"/>
    public SvgElement Convert(Element element)
    {
        return element switch
        {
            ModelCurve curve => new ModelCurveToSvg().Convert(curve),
            Panel panel => new PanelToSvg().Convert(panel),
            // TODO: Implement ModelTextToSvg conversion.
            _ => throw new TypeSwitchException<Element>("Failed to convert element to SVG", element)
        };
    }
}
