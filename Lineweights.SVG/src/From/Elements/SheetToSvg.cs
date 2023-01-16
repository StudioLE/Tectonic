using Lineweights.Drawings;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="Sheet"/> to a <see cref="SvgDocument"/>
/// </summary>
internal sealed class SheetToSvg : IConverter<Sheet, SvgElement>
{
    /// <inheritdoc cref="SheetToSvg" />
    public SvgElement Convert(Sheet sheet)
    {
        SvgElement svgElement = new("svg");
        svgElement.RemoveAttributes();
        svgElement.Add(SvgHelpers.ViewBoxAttribute(sheet));

        ElementToSvg converter = new();
        ParallelQuery<SvgElement> childElements = sheet
            .Render()
            .AsParallel()
            .AsOrdered()
            .Select(converter.Convert);

        svgElement.Add(childElements);
        return svgElement;
    }
}
