using Geometrician.Drawings;

namespace Geometrician.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="View"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class ViewToSvg : IConverter<View, SvgElement>
{
    /// <inheritdoc cref="ViewToSvg"/>
    public SvgElement Convert(View view)
    {
        SvgElement svgElement = new("svg");
        svgElement.RemoveAttributes();
        svgElement.Add(SvgHelpers.ViewBoxAttribute(view));

        ElementToSvg converter = new();
        ParallelQuery<SvgElement> childElements = view
            .Render()
            .Select(converter.Convert);

        svgElement.Add(childElements);
        return svgElement;
    }
}
