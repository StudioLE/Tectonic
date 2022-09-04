using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Drawings.Tests;

[VisualizeAfterTest]
internal sealed class ViewBuilderTests
{
    private readonly Model _model = new();
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();

    [SetUp]
    public void Setup()
    {
        _model.AddElements(CreateModelArrows.ByTransform(new()));
        _model.AddElements(Scenes.Brickwork());
    }

    [TestCase(ViewDirection.Back)]
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
        _model.AddElements(view.Scope.Border);
        _model.AddElements(cornerArrows);
        _model.AddElements(originArrows);
        _model.AddElements(view);
        //Model.AddElements(view.ToBox().ToModelCurves(MaterialByName("Orange")));

        // Assert
        Verify.Geometry(view.Scope.ToBox());
    }
}
