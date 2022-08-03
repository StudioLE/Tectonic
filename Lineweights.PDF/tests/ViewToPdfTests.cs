using System.IO;
using QuestPDF.Fluent;
using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;
using Lineweights.Drawings.Rendering;

namespace Lineweights.PDF.Tests;

internal sealed class ViewToPdfTests
{
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
        var converter = new ViewToPdf();
        PdfDocument pdfDocument = converter.Convert(view);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
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
        var converter = new ViewToPdf();
        PdfDocument pdfDocument = converter.Convert(view);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
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
        var converter = new ViewToPdf();
        PdfDocument pdfDocument = converter.Convert(view);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
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
        var converter = new ViewToPdf();
        PdfDocument pdfDocument = converter.Convert(view);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
    }
}
