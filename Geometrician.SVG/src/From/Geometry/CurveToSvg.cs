using static Geometrician.SVG.SvgHelpers;

namespace Geometrician.SVG.From.Geometry;

/// <summary>
/// Convert a <see cref="Curve"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class CurveToSvg : IConverter<Curve, SvgElement>
{
    /// <inheritdoc cref="CurveToSvg"/>
    public SvgElement Convert(Curve curve)
    {
        SvgElement svgElement = curve switch
        {
            Circle circle => new CircleToSvg().Convert(circle),
            Arc arc => throw new NotImplementedException(),
            Bezier bezier => throw new NotImplementedException(),
            Line line => new LineToSvg().Convert(line),
            Polygon polygon => new PolygonToSvg().Convert(polygon),
            Polyline polyline => new PolylineToSvg().Convert(polyline),
            _ => throw new ArgumentOutOfRangeException()
        };

        return svgElement;
    }
}

/// <summary>
/// Convert a <see cref="Curve"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class CircleToSvg : IConverter<Circle, SvgElement>
{
    /// <inheritdoc/>
    /// <remarks>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Element/circle">SVG circle documentation</see>
    /// </remarks>
    public SvgElement Convert(Circle circle)
    {
        return new("circle ",
            new XAttribute("cx", circle.Center.X),
            new XAttribute("cy", circle.Center.Y),
            new XAttribute("cr", circle.Radius));
    }
}

/// <summary>
/// Convert a <see cref="Line"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class LineToSvg : IConverter<Line, SvgElement>
{
    /// <inheritdoc cref="LineToSvg"/>
    /// <remarks>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Element/line">SVG line documentation</see>
    /// </remarks>
    public SvgElement Convert(Line line)
    {
        return new("line", CoordinateAttributes(line.Start, line.End));
    }
}

/// <summary>
/// Convert a <see cref="Polyline"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class PolylineToSvg : IConverter<Polyline, SvgElement>
{
    /// <inheritdoc cref="PolylineToSvg"/>
    /// <remarks>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Element/polyline">SVG polyline documentation</see>
    /// </remarks>
    public SvgElement Convert(Polyline polyline)
    {
        return new("polyline", PointsAttribute(polyline.Vertices));
    }
}

/// <summary>
/// Convert a <see cref="Polygon"/> to a <see cref="SvgElement"/>
/// </summary>
internal sealed class PolygonToSvg : IConverter<Polygon, SvgElement>
{
    /// <inheritdoc cref="PolygonToSvg"/>
    /// <remarks>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Element/polygon">SVG polygon documentation</see>
    /// </remarks>
    public SvgElement Convert(Polygon polygon)
    {
        return new("polygon",
            PointsAttribute(polygon.Vertices.Append(polygon.Vertices.First())));
    }
}
