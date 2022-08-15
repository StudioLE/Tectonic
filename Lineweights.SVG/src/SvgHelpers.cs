using Lineweights.Drawings;
using StudioLE.Core.System;

namespace Lineweights.SVG;

/// <summary>
/// Methods to help construct <see cref="SvgElement"/>.
/// </summary>
internal static class SvgHelpers
{
    /// <summary>
    /// Convert <paramref name="vertices"/> to SVG
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Element/polyline">polyline points attribute</see>.
    /// </summary>
    internal static XAttribute PointsAttribute(IEnumerable<Vector3> vertices)
    {
        string value = string.Join(" ", vertices.Select(v => $"{v.X.Round()},{v.Y.InvertY().Round()}"));
        return new("points", value);
    }

    /// <summary>
    /// Convert <paramref name="vertices"/> to SVG
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Element/line">line coordinate attributes</see>.
    /// </summary>
    internal static IEnumerable<XAttribute> CoordinateAttributes(params Vector3[] vertices)
    {
        return vertices
            .SelectMany((v, i) => new[]
            {
                new XAttribute("x" + (i + 1), v.X.Round()),
                new XAttribute("y" + (i + 1), v.Y.InvertY().Round())
            });
    }

    /// <summary>
    /// Convert <paramref name="canvas"/> to SVG
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/viewBox">viewBox attribute</see>.
    /// </summary>
    internal static XAttribute ViewBoxAttribute(Canvas canvas)
    {
        double width = canvas.Width.Round();
        double height = canvas.Height.Round();
        double minX = width * 0.5 * -1;
        double minY = height * 0.5 * -1;
        return new("viewBox", $"{minX} {minY} {width} {height}");
    }

    /// <summary>
    /// Ensure the Y axis is inverted because in SVG the Y axis is positive when moving down the page.
    /// </summary>
    internal static double InvertY(this double @this)
    {
        return @this * -1;
    }

    /// <summary>
    /// Round the values to a given precision to avoid floating points.
    /// </summary>
    internal static double Round(this double @this)
    {
        return @this.Round(5);
    }
}
