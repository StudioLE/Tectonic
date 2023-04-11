using Geometrician.Drawings;

namespace Geometrician.PDF.From.Elements;

/// <summary>
/// Convert a <see cref="View"/>to PDF
/// by rendering it and drawing the 2d representation on the <see cref="SKCanvas"/>.
/// </summary>
internal sealed class ViewToPdf : IConverter<View, PdfView>
{
    /// <inheritdoc cref="ViewToPdf"/>
    public PdfView Convert(View view)
    {
        return new(view);
    }
}
