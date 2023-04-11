using Geometrician.Diagnostics.NUnit.Verification;
using Geometrician.Diagnostics.Samples;
using Geometrician.Flex;

namespace Geometrician.Drawings.Tests;

internal sealed class SerializationTests
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
    public void Serialization_View()
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
        VerifyHelpers.SerializationAsModel(view);
    }

    [Test]
    public void Serialization_ViewScope()
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(ViewDirection.Top)
            .ElementsInView(_geometry);
        View view = builder.Build();

        // Act
        VerifyHelpers.SerializationAsModel(view.Scope);
    }

    [Test]
    public void Serialization_SheetContent()
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
        VerifyHelpers.SerializationAsModel(sheet.Content);
    }

    [Test]
    public void Serialization_Sheet()
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
        VerifyHelpers.SerializationAsModel(expectedSheet);
    }
}
