using System.IO;
using Geometrician.Core.Assets;
using Geometrician.Diagnostics.NUnit;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Diagnostics.Samples;
using Geometrician.Drawings;
using Geometrician.Drawings.Rendering;
using Geometrician.PDF.From.Elements;
using NUnit.Framework;
using QuestPDF.Fluent;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.PDF.Tests;

internal sealed class ViewToPdfTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private Model _model = new();

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
        await _verify.File(file);
        Preview(file);
    }

    private void Preview(FileInfo file)
    {
        _model.AddElement(new ExternalAsset { Location = new(file.FullName) });
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model);
        _model = new();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
