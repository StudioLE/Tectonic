using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.PDF.From.Elements;
using Lineweights.Workflows.NUnit.Visualization;
using QuestPDF.Fluent;

namespace Lineweights.PDF.Tests;

internal sealed class ViewToPdfTests
{
    private readonly Model _model = new();

    [TestCase(ViewDirection.Back)]
    public async Task ViewToPdf_Wireframe_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.Brickwork());
        View view = builder.Build();

        // Act
        ViewToPdf converter = new();
        PdfDocument pdfDocument = converter.Convert(view);

        // Assert
        await VerifyPdf(pdfDocument);
    }

    [TestCase(ViewDirection.Back)]
    public async Task ViewToPdf_Wireframe_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.GeometricElements());
        View view = builder.Build();

        // Act
        ViewToPdf converter = new();
        PdfDocument pdfDocument = converter.Convert(view);

        // Assert
        await VerifyPdf(pdfDocument);
    }
    [TestCase(ViewDirection.Back)]
    public async Task ViewToPdf_Fill_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.Brickwork())
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        ViewToPdf converter = new();
        PdfDocument pdfDocument = converter.Convert(view);

        // Assert
        await VerifyPdf(pdfDocument);
    }

    [TestCase(ViewDirection.Back)]
    public async Task ViewToPdf_Fill_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.GeometricElements())
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        ViewToPdf converter = new();
        PdfDocument pdfDocument = converter.Convert(view);

        // Preview
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

    [TearDown]
    public async Task TearDown()
    {
        await new Visualize().Execute(_model);
    }
}
