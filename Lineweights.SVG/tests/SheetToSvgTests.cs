using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using Lineweights.Drawings.Rendering;
using Lineweights.Flex;
using Lineweights.SVG.From.Elements;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.SVG.Tests;

[VisualizeAfterTest]
internal sealed class SheetToSvgTests
{
    private readonly Model _model = new();
    private readonly SheetToSvg _converter = new();
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
        SvgDocument svgDocument = _converter.Convert(sheet);

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
        SvgDocument svgDocument = _converter.Convert(sheet);

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
        SvgDocument svgDocument = _converter.Convert(sheet);

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
        SvgDocument svgDocument = _converter.Convert(sheet);

        // Asset
        await VerifySvg(svgDocument);
    }

    private async Task VerifySvg(SvgDocument svgDocument)
    {
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
