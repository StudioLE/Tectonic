using System.IO;
using StudioLE.Core.Exceptions;
using Lineweights.Drawings;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="Canvas"/> to a PDF and save it to the context path.
/// </summary>
public sealed class CanvasToPdfFile : IConverter<Canvas, FileInfo, FileInfo>
{
    /// <inheritdoc cref="CanvasToPdfFile"/>
    public FileInfo Convert(Canvas canvas, FileInfo file)
    {
        PdfDocument pdfDocument = canvas switch
        {
            Sheet sheet => new SheetToPdf().Convert(sheet),
            View view => new ViewToPdf().Convert(view),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to pdf file.", canvas)
        };
        pdfDocument.GeneratePdf(file.FullName);
        return file;
    }
}
