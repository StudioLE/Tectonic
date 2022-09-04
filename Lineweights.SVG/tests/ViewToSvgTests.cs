using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.SVG.From.Elements;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.SVG.Tests;

[VisualizeAfterTest]
internal sealed class ViewToSvgTests
{
    private readonly Model _model = new();
    private readonly ViewToSvg _converter = new();

    [TestCase(ViewDirection.Back)]
    public async Task ViewToSvg_Wireframe_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.Brickwork());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);

        // Asset
        await VerifySvg(svgElement);
    }

    [TestCase(ViewDirection.Back)]
    public async Task ViewToSvg_Wireframe_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.GeometricElements());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);

        // Asset
        await VerifySvg(svgElement);
    }
    [TestCase(ViewDirection.Back)]
    public async Task ViewToSvg_Flat_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.Brickwork())
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);

        // Asset
        await VerifySvg(svgElement);
    }

    [TestCase(ViewDirection.Back)]
    public async Task ViewToSvg_Flat_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(Scenes.GeometricElements())
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(view);

        // Asset
        await VerifySvg(svgElement);
    }

    private async Task VerifySvg(SvgElement svgElement)
    {
        SvgDocument svgDocument = new();
        svgDocument.Add(svgElement);
        FileInfo file = TestHelpers.FileByTestContext("svg");
        svgDocument.Save(file.FullName);
        await Verify.File(file);
        Preview(file);
    }

    private void Preview(FileInfo file)
    {
        _model.AddElement(new DocumentInformation { Location = new(file.FullName) });
    }
}
