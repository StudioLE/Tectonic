using Lineweights.Drawings;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="Sheet"/> to a <see cref="SvgDocument"/>
/// </summary>
internal sealed class SheetToSvg : IConverter<Sheet, SvgDocument>
{
    /// <inheritdoc cref="SheetToSvg" />
    public SvgDocument Convert(Sheet sheet)
    {
        SvgElement svgElement = new("svg");
        svgElement.RemoveAttributes();
        svgElement.Add(SvgHelpers.ViewBoxAttribute(sheet));

        var converter = new ElementToSvg();
        ParallelQuery<SvgElement> childElements = sheet
            .Render()
            .AsParallel()
            .AsOrdered()
            .Select(converter.Convert);

        svgElement.Add(childElements);

        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        return svgDocument;
    }
}
