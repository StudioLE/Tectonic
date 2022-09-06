using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.Flex;
using Lineweights.PDF.From.Elements;
using Lineweights.Workflows.NUnit.Visualization;
using QuestPDF.Fluent;

namespace Lineweights.PDF.Tests;

[VisualizeAfterTest]
internal sealed class SheetToPdfTests
{
    private readonly Model _model = new();
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();
    private ISequenceBuilder _sequenceBuilder = default!;
    private IDistribution2dBuilder _defaultViewArrangement = default!;

    [SetUp]
    public void Setup()
    {
        _sequenceBuilder = new SequenceBuilder();
        _defaultViewArrangement = new DefaultViewArrangement();
    }

    [Test]
    public async Task SheetToPdf_Wireframe_Brickwork()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .Scale(1d / 10)
            .ElementsInView(_brickwork);

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        ISheetBuilder builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = (Sheet)builder.Build();

        // Act
        SheetToPdf converter = new();
        PdfSheet pdfDocument = converter.Convert(sheet);

        // Assert
        await VerifyPdf(pdfDocument);
    }

    [Test]
    public async Task SheetToPdf_Wireframe_GeometricElements()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Back,
            ViewDirection.Front,
            ViewDirection.Left,
            ViewDirection.Right
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.100, .100, .100)
            .Scale(1d / 5)
            .ElementsInView(_geometry);

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        ISheetBuilder builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = (Sheet)builder.Build();

        // Act
        SheetToPdf converter = new();
        PdfDocument pdfDocument = converter.Convert(sheet);

        // Assert
        await VerifyPdf(pdfDocument);
    }

    [Test]
    public async Task SheetToPdf_Flat_Brickwork()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Front,
            ViewDirection.Back
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .Scale(1d / 10)
            .ElementsInView(_brickwork)
            .RenderStrategy(new FlatRender());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        ISheetBuilder builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = (Sheet)builder.Build();

        // Act
        SheetToPdf converter = new();
        PdfSheet pdfDocument = converter.Convert(sheet);

        // Assert
        await VerifyPdf(pdfDocument);
    }

    [Test]
    public async Task SheetToPdf_Flat_GeometricElements()
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Back,
            ViewDirection.Front,
            ViewDirection.Left,
            ViewDirection.Right
        };

        ViewBuilder viewBuilder = new ViewBuilder()
            .ScopePadding(.100, .100, .100)
            .Scale(1d / 5)
            .ElementsInView(_geometry)
            .RenderStrategy(new FlatRender());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        ISheetBuilder builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = (Sheet)builder.Build();

        // Act
        SheetToPdf converter = new();
        PdfDocument pdfDocument = converter.Convert(sheet);

        // Assert
        await VerifyPdf(pdfDocument);
    }

    private async Task VerifyPdf(PdfDocument pdfDocument)
    {
        pdfDocument.Metadata.CreationDate = DateTime.UnixEpoch;
        pdfDocument.Metadata.ModifiedDate = DateTime.UnixEpoch;
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);
        await Verify.File(file);
        Preview(file);
    }

    private void Preview(FileInfo file)
    {
        _model.AddElement(new DocumentInformation { Location = new(file.FullName) });
    }
}
