using System.IO;
using Lineweights.Core.Assets;
using Lineweights.Diagnostics.NUnit;
using Lineweights.Diagnostics.NUnit.Visualization;
using Lineweights.Diagnostics.Samples;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.Flex;
using Lineweights.SVG.From.Elements;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Lineweights.SVG.Tests;

internal sealed class SheetToSvgTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private readonly SheetToSvg _converter = new();
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();
    private Model _model = new();
    private ISequenceBuilder _sequenceBuilder = default!;
    private IDistribution2dBuilder _defaultViewArrangement = default!;

    [SetUp]
    public void Setup()
    {
        _sequenceBuilder = new SequenceBuilder();
        _defaultViewArrangement = new DefaultViewArrangement();
    }

    [Test]
    public async Task SheetToSvg_Wireframe_Brickwork()
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

        IBuilder<Sheet> builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(sheet);
        SvgDocument svgDocument = new(svgElement);

        // Asset
        await VerifySvg(svgDocument);
    }

    [Test]
    public async Task SheetToSvg_Wireframe_GeometricElements()
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
            .ScopePadding(.050, .050, .050)
            .Scale(1d / 5)
            .ElementsInView(_geometry);

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        IBuilder<Sheet> builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(sheet);
        SvgDocument svgDocument = new(svgElement);

        // Asset
        await VerifySvg(svgDocument);
    }

    [Test]
    public async Task SheetToSvg_Flat_Brickwork()
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

        IBuilder<Sheet> builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(sheet);
        SvgDocument svgDocument = new(svgElement);

        // Asset
        await VerifySvg(svgDocument);
    }

    [Test]
    public async Task SheetToSvg_Flat_GeometricElements()
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
            .ScopePadding(.050, .050, .050)
            .Scale(1d / 5)
            .ElementsInView(_geometry)
            .RenderStrategy(new FlatRender());

        View[] views = viewDirections
            .Select(direction => viewBuilder
                .ViewDirection(direction)
                .Build())
            .ToArray();

        IBuilder<Sheet> builder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);
        Sheet sheet = builder.Build();

        // Act
        SvgElement svgElement = _converter.Convert(sheet);
        SvgDocument svgDocument = new(svgElement);

        // Asset
        await VerifySvg(svgDocument);
    }

    private async Task VerifySvg(SvgDocument svgDocument)
    {
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
