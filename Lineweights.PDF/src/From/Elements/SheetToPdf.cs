using Lineweights.Drawings;

namespace Lineweights.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="Sheet"/>to PDF
/// by rendering it and drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class SheetToPdf : IConverter<Sheet, PdfSheet>
{
    /// <inheritdoc cref="SheetToPdf" />
    public PdfSheet Convert(Sheet sheet)
    {
        return new(sheet);
    }
}
