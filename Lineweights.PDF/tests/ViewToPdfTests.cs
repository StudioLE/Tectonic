using System.IO;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.PDF.From.Elements;
using Lineweights.Workflows.NUnit.Visualization;
using QuestPDF.Fluent;

namespace Lineweights.PDF.Tests;

[VisualizeInServerAppAfterTest]
internal sealed class ViewToPdfTests
{
    private readonly Model _model = new();

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToPdf_Wireframe_Brickwork(ViewDirection viewDirection)
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

        // Preview
        Preview(pdfDocument);
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToPdf_Wireframe_GeometricElements(ViewDirection viewDirection)
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

        // Preview
        Preview(pdfDocument);
    }
    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToPdf_Fill_Brickwork(ViewDirection viewDirection)
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

        // Preview
        Preview(pdfDocument);
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewToPdf_Fill_GeometricElements(ViewDirection viewDirection)
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
        Preview(pdfDocument);
    }

    private void Preview(PdfDocument pdfDocument)
    {
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);
        _model.AddElement(new DocumentInformation { Location = new(file.FullName) });
    }
}
