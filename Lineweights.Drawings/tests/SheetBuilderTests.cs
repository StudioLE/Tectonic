using Lineweights.Flex;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Drawings.Tests;

[VisualizeAfterTest]
internal sealed class SheetBuilderTests
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
        _model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(Justification.Start)]
    [TestCase(Justification.SpaceEvenly)]
    public async Task SheetBuilder_Justification_Brickwork(Justification mainJustification, Justification? crossJustification = null)
    {
        // Arrange
        ViewDirection[] viewDirections =
        {
            ViewDirection.Top,
            ViewDirection.Bottom,
            ViewDirection.Left,
            ViewDirection.Right,
            ViewDirection.Back,
            ViewDirection.Front,
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

        IBuilder<Sheet> sheetBuilder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                if (viewArrangement is not Flex2d flex)
                    throw new NotSupportedException();
                flex.MainJustification(mainJustification);
                flex.CrossJustification(crossJustification ?? mainJustification);
            });

        // Act
        Sheet sheet = (Sheet)sheetBuilder.Build();
        GeometricElement[] geometry = sheet.Render().ToArray();

        // Preview
        _model.AddElements(geometry);

        // Assert
        await Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(Justification.Start)]
    [TestCase(Justification.SpaceEvenly)]
    public async Task SheetBuilder_Justification_GeometricElements(Justification mainJustification, Justification? crossJustification = null)
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

        IBuilder<Sheet> sheetBuilder = new SheetBuilder(_sequenceBuilder, _defaultViewArrangement)
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                if (viewArrangement is not Flex2d flex)
                    throw new NotSupportedException();
                flex.MainJustification(mainJustification);
                flex.CrossJustification(crossJustification ?? mainJustification);
            });

        // Act
        Sheet sheet = (Sheet)sheetBuilder.Build();
        GeometricElement[] geometry = sheet.Render().ToArray();

        // Preview
#if false
        IEnumerable<Element> elements = Enumerable.Empty<Element>()
            .Concat(_geometry)
            .Concat(views);
        // TODO: View seems to work but not sheet? Possibly due to sheet.Content.Views?
            //.Append(sheet)
            //.Concat(sheet.Content.Views); // TODO: Sub sub content isn't gathered by Model.ToJson().
        Scenes.ToJson(Scenes.Name.GeometricElementsOnSheet, elements);
#endif
        _model.AddElements(geometry);

        // Assert
        await Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }
}
