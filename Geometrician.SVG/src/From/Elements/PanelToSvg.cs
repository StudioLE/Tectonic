using Geometrician.SVG.From.Geometry;

namespace Geometrician.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="Panel"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class PanelToSvg : IConverter<Panel, SvgElement>
{
    /// <inheritdoc cref="PanelToSvg"/>
    public SvgElement Convert(Panel element)
    {
        Polygon polygon = element.Perimeter.TransformedPolygon(element.Transform);
        SvgElement svgElement = new PolygonToSvg().Convert(polygon);

        svgElement.Add(new XAttribute("stroke", "#ffffff"));
        svgElement.Add(new XAttribute("stroke-width", .001));
        svgElement.Add(new XAttribute("fill", element.Material.Color.ToHex()));

        return svgElement;
    }
}
