using Geometrician.SVG.From.Geometry;

namespace Geometrician.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="ModelCurve"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class ModelCurveToSvg : IConverter<ModelCurve, SvgElement>
{
    /// <inheritdoc cref="ModelCurveToSvg"/>
    public SvgElement Convert(ModelCurve element)
    {
        Curve curve = element.Curve.Transformed(element.Transform);
        SvgElement svgElement = new CurveToSvg().Convert(curve);

        svgElement.Add(new XAttribute("stroke", element.Material.Color.ToHex()));
        svgElement.Add(new XAttribute("stroke-width", .001));
        svgElement.Add(new XAttribute("fill", "none"));

        return svgElement;
    }
}
