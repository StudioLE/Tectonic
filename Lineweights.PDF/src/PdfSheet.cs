using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;

namespace Lineweights.PDF;

/// <summary>
/// A <see cref="PdfDocument"/> representing a <see cref="Sheet"/>.
/// </summary>
public sealed class PdfSheet : PdfDocument
{
    private readonly Sheet _sheet;

    /// <inheritdoc cref="PdfSheet"/>
    public PdfSheet(Sheet sheet) : base(sheet)
    {
        _sheet = sheet;
    }

    /// <inheritdoc />
    protected override void ComposeLayers(LayersDescriptor layers)
    {
        bool isTitleHorizontal = _sheet.Title.Width.ApproximatelyEquals(_sheet.Width);

        layers
            .Layer()
            .PaddingLeft(isTitleHorizontal ? 0 : (float)_sheet.Content.Width, Unit.Meter)
            .PaddingTop(isTitleHorizontal ? (float)_sheet.Content.Height : 0, Unit.Meter)
            .Width((float)_sheet.Title.Width, Unit.Meter)
            .Height((float)_sheet.Title.Height, Unit.Meter)
            //.Background(Colors.Grey.Lighten2)
            .Element(ComposeSheetTitle);

        layers
            .PrimaryLayer()
            .PaddingRight(isTitleHorizontal ? 0 : (float)_sheet.Title.Width, Unit.Meter)
            .PaddingBottom(isTitleHorizontal ? (float)_sheet.Title.Height : 0, Unit.Meter)
            .Width((float)_sheet.Content.Width, Unit.Meter)
            .Height((float)_sheet.Content.Height, Unit.Meter)
            //.Background(Colors.Grey.Lighten3)
            .Element(ComposeSheetContent);

        layers.Layer()
            .Canvas((canvas, size) => new CanvasToPdf().Convert(_sheet, canvas, size));
    }

    /// <summary>
    /// Compose the sheet title.
    /// </summary>
    /// <remarks>
    /// Note: When converting the <see cref="Sheet"/> using <see cref="CanvasToPdf"/> the <see cref="Sheet.Title"/>
    /// is rendered on the canvas layer so it is not necessary to compose anything here.
    /// </remarks>
    private static void ComposeSheetTitle(IContainer container)
    {
        //container.Text("Title");
    }

    /// <summary>
    /// Compose the sheet content.
    /// </summary>
    /// <remarks>
    /// Note: When converting the <see cref="Sheet"/> using <see cref="CanvasToPdf"/> the <see cref="Sheet.Title"/>
    /// is rendered on the canvas layer so it is not necessary to compose anything here.
    /// </remarks>
    private static void ComposeSheetContent(IContainer container)
    {
        //container.Text("Content");
    }
}
