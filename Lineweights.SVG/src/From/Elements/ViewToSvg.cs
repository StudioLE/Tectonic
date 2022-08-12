using Lineweights.Drawings;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="View"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class ViewToSvg : IConverter<View, SvgElement>
{
    /// <inheritdoc cref="ViewToSvg"/>
    public SvgElement Convert(View view)
    {
        var svgElement = new SvgElement("svg");
        svgElement.RemoveAttributes();
        svgElement.Add(SvgHelpers.ViewBoxAttribute(view));

        var converter = new ElementToSvg();
        ParallelQuery<SvgElement> childElements = view
            .Render()
            .Select(converter.Convert);

        svgElement.Add(childElements);
        return svgElement;
    }
}
