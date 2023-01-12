using Lineweights.Diagnostics.NUnit.Verification;
using Lineweights.Diagnostics.Samples;
using Lineweights.Flex;

namespace Lineweights.Drawings.Tests;

internal sealed class SerialisationTests
{
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
    public void Serialisation_View()
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(ViewDirection.Top)
            .ElementsInView(_geometry);
        View view = builder.Build();
        Model model = new();
        model.AddElements(view);

        // Act
        VerifyHelpers.SerialisationAsModel(view);
    }

    [Test]
    public void Serialisation_ViewScope()
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(ViewDirection.Top)
            .ElementsInView(_geometry);
        View view = builder.Build();

        // Act
        VerifyHelpers.SerialisationAsModel(view.Scope);
    }

    [Test]
    public void Serialisation_SheetContent()
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
            .Views(views);

        Sheet sheet = sheetBuilder.Build();

        // Act
        VerifyHelpers.SerialisationAsModel(sheet.Content);
    }

    [Test]
    public void Serialisation_Sheet()
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
            .Views(views);

        Sheet expectedSheet = sheetBuilder.Build();

        // Act
        VerifyHelpers.SerialisationAsModel(expectedSheet);
    }
}
