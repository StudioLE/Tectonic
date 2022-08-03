using Lineweights.Flex;

namespace Lineweights.Drawings.Tests;

[SendToDashboardAfterTest]
internal sealed class SheetBuilderTests : ResultModel
{
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

    [SetUp]
    public void Setup()
    {
        Model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(Justification.Start)]
    [TestCase(Justification.SpaceEvenly)]
    public void SheetBuilder_Justification_Brickwork(Justification mainJustification, Justification? crossJustification = null)
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
                // TODO: Set Name
                .Build())
            .ToArray();

        SheetBuilder sheetBuilder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(mainJustification);
                viewArrangement.CrossJustification(crossJustification ?? mainJustification);
            });

        // Act
        Sheet sheet = sheetBuilder.Build();
        GeometricElement[] geometry = sheet.Render().ToArray();

        // Preview
        Model.AddElements(geometry);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(Justification.Start)]
    [TestCase(Justification.SpaceEvenly)]
    public void SheetBuilder_Justification_GeometricElements(Justification mainJustification, Justification? crossJustification = null)
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
                // TODO: Set Name
                .Build())
            .ToArray();

        SheetBuilder sheetBuilder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views)
            .ViewArrangement(viewArrangement =>
            {
                viewArrangement.MainJustification(mainJustification);
                viewArrangement.CrossJustification(crossJustification ?? mainJustification);
            });

        // Act
        Sheet sheet = sheetBuilder.Build();
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
        Model.AddElements(geometry);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }
}
