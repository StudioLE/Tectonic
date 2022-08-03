using System.IO;
using StudioLE.Core.Exceptions;
using Lineweights.Drawings;

namespace Lineweights.SVG.From.Elements;

/// <summary>
/// Convert a <see cref="Canvas"/> to a SVG and save it to the context path.
/// </summary>
public sealed class CanvasToSvgFile : IConverter<Canvas, FileInfo, FileInfo>
{
    /// <inheritdoc cref="CanvasToSvgFile"/>
    public FileInfo Convert(Canvas canvas, FileInfo file)
    {
        SvgDocument svgDocument = canvas switch
        {
            // TODO: If new() works for View then we should amend SheetToSvg to also return the SVG element.
            Sheet sheet => new SheetToSvg().Convert(sheet),
            View view => new(new ViewToSvg().Convert(view)),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to pdf file.", canvas)
        };
        svgDocument.Save(file.FullName, SaveOptions.None);
        return file;
    }
}
