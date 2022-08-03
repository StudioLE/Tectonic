namespace Lineweights.Drawings.Tests;

[SendToDashboardAfterTest]
internal sealed class ViewBuilderTests : ResultModel
{
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();

    [SetUp]
    public void Setup()
    {
        Model.AddElements(CreateModelArrows.ByTransform(new()));
        Model.AddElements(Scenes.Brickwork());
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void ViewBuilder_ViewDirection(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(_brickwork);

        // Act
        View view = builder.Build();

        // Preview
        IEnumerable<ModelArrows> cornerArrows = view
            .Scope
            .Border
            .Vertices
            .Select(origin => CreateModelArrows
                .ByVectors(origin, view.Scope.FacingDirection, view.Scope.Depth, Colors.Blue));
        IEnumerable<ModelArrows> originArrows = new[]
        {
            CreateModelArrows.ByVectors(view.Scope.Origin, view.Scope.RightDirection, view.Width / 2, Colors.Red),
            CreateModelArrows.ByVectors(view.Scope.Origin, view.Scope.UpDirection, view.Height / 2, Colors.Green),
            CreateModelArrows.ByVectors(view.Scope.Origin, view.Scope.FacingDirection, view.Scope.Depth, Colors.Blue)
        };
        Model.AddElements(view.Scope.Border);
        Model.AddElements(cornerArrows);
        Model.AddElements(originArrows);
        Model.AddElements(view);
        //Model.AddElements(view.ToBox().ToModelCurves(MaterialByName("Orange")));

        // Assert
        Verify.Geometry(view.Scope.ToBox());
    }
}
