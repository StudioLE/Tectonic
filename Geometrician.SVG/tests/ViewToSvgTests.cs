using System.IO;
using Geometrician.Core.Assets;
using Geometrician.Diagnostics.NUnit;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Diagnostics.Samples;
using Geometrician.Drawings;
using Geometrician.Drawings.Rendering;
using Geometrician.SVG.From.Elements;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.SVG.Tests;

internal sealed class ViewToSvgTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private readonly ViewToSvg _converter = new();
    private Model _model = new();

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
