using Geometrician.Drawings;
using QuestPDF.Drawing;
using Colors = QuestPDF.Helpers.Colors;

namespace Geometrician.PDF;

/// <summary>
/// A PDF document template.
/// Follows the <see href="https://refactoring.guru/design-patterns/template-method">template method</see> pattern.
/// </summary>
public abstract class PdfDocument : IDocument
{
    private readonly Canvas _canvas;

    /// <summary>
    /// The metadata of the document.
    /// </summary>
    public DocumentMetadata Metadata { get; set; } = DocumentMetadata.Default;

    /// <inheritdoc cref="PdfDocument"/>
    protected PdfDocument(Canvas canvas)
    {
        _canvas = canvas;
    }

    /// <inheritdoc/>
    public DocumentMetadata GetMetadata()
    {
        return Metadata;
    }

    /// <summary>
    /// Compose the document.
    /// </summary>
    public virtual void Compose(IDocumentContainer doc)
    {
        doc.Page(ComposePage);
    }

    /// <summary>
    /// Compose the page within the document.
    /// </summary>
    protected virtual void ComposePage(PageDescriptor page)
    {
        page.Size((float)_canvas.Width, (float)_canvas.Height, Unit.Meter);
        page.PageColor(Colors.Transparent);
        page.Content().Layers(ComposeLayers);
    }

    /// <summary>
    /// Compose the layers within the page.
    /// </summary>
    protected abstract void ComposeLayers(LayersDescriptor layers);
}
