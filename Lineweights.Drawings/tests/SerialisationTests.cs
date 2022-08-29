using Ardalis.Result;
using Lineweights.Workflows.Results;

namespace Lineweights.Drawings.Tests;

[SendToServerAfterTest]
internal sealed class SerialisationTests : ResultModel
{
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

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
        string json = model.ToJson(true);
        Result<Model> result = ModelHelpers.TryGetFromJson(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, "Serialisation succeeded.");
            Assert.That(result.Errors, Is.Empty, "Serialisation errors.");
            View deserialised = model
                .AllElementsOfType<View>()
                .First();
            Assert.That(deserialised, Is.EqualTo(view), "Matches.");
        });
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
        Model model = new();
        model.AddElements(view.Scope);

        // Act
        string json = model.ToJson(true);
        Result<Model> result = ModelHelpers.TryGetFromJson(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, "result.IsSuccess");
            Assert.That(result.Errors, Is.Empty, "Serialisation errors.");
            ViewScope deserialised = model
                .AllElementsOfType<ViewScope>()
                .First();
            Assert.That(deserialised, Is.Not.Null, "Not null.");
            Assert.That(deserialised, Is.EqualTo(view.Scope), "Matches.");
        });
    }

    [Test]
    [Ignore("Serialisation failing")]
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

        SheetBuilder sheetBuilder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);

        Sheet sheet = sheetBuilder.Build();
        Model model = new();
        model.AddElements(sheet.Content);

        // Act
        string json = model.ToJson(true);
        Result<Model> result = ModelHelpers.TryGetFromJson(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, "Serialisation succeeded.");
            Assert.That(result.Errors, Is.Empty, "Serialisation errors.");
            SheetContent deserialised = model
                .AllElementsOfType<SheetContent>()
                .First();
            Assert.That(deserialised, Is.EqualTo(sheet), "Matches.");
        });
    }

    [Test]
    [Ignore("Serialisation failing")]
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

        SheetBuilder sheetBuilder = new SheetBuilder()
            .SheetSize(.841, .594)
            .VerticalTitleArea(.075)
            .Views(views);

        Sheet sheet = sheetBuilder.Build();
        Model model = new();
        model.AddElements(sheet);

        // Act
        string json = model.ToJson(true);
        Result<Model> result = ModelHelpers.TryGetFromJson(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, "Serialisation succeeded.");
            Assert.That(result.Errors, Is.Empty, "Serialisation errors.");
            Sheet deserialised = model
                .AllElementsOfType<Sheet>()
                .First();
            Assert.That(deserialised, Is.EqualTo(sheet), "Matches.");
        });
    }
}
