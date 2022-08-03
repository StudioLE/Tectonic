using System.IO;
using QuestPDF.Fluent;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.PDF.From.Elements;
using Lineweights.Flex;

namespace Lineweights.PDF.Tests;

internal sealed class SheetToPdfTests
{
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

    [Test]
    public void SheetToPdf_Wireframe_Brickwork()
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
                // TODO: Set Name
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        var converter = new SheetToPdf();
        PdfSheet pdfDocument = converter.Convert(sheet);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
    }

    [Test]
    public void SheetToPdf_Wireframe_GeometricElements()
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
                // TODO: Set Name
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        var converter = new SheetToPdf();
        PdfDocument pdfDocument = converter.Convert(sheet);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
    }

    [Test]
    public void SheetToPdf_Flat_Brickwork()
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
                // TODO: Set Name
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        var converter = new SheetToPdf();
        PdfSheet pdfDocument = converter.Convert(sheet);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
    }

    [Test]
    public void SheetToPdf_Flat_GeometricElements()
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
                // TODO: Set Name
                .Build())
            .ToArray();

        SheetBuilder builder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(Justification.SpaceEvenly);
                viewArrangement.CrossJustification(Justification.SpaceEvenly);
            });
        Sheet sheet = builder.Build();

        // Act
        var converter = new SheetToPdf();
        PdfDocument pdfDocument = converter.Convert(sheet);
        FileInfo file = TestHelpers.FileByTestContext("pdf");
        pdfDocument.GeneratePdf(file.FullName);

        // Preview
        //TestHelpers.OpenInDebug(file);

        // Assert
        //Verify.File(file);
    }
}
