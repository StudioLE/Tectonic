using Lineweights.Drawings.Rendering;
using Lineweights.Workflows.Results;

namespace Lineweights.Drawings.Tests;

[SendToServerAfterTest]
internal sealed class ViewTests : ResultModel
{
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

    [SetUp]
    public void Setup()
    {
        Model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void View_Render_Wireframe_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(_brickwork);
        View view = builder.Build();

        // Act
        GeometricElement[] geometry = view.Render().ToArray();

        // Preview
        Model.AddElements(geometry);
        Model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void View_Render_Wireframe_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(_geometry);
        View view = builder.Build();

        // Act
        GeometricElement[] geometry = view.Render().ToArray();

        // Preview
        Model.AddElements(geometry);
        Model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void View_RenderInPlace_Wireframe_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(_brickwork);
        View view = builder.Build();

        // Act
        GeometricElement[] geometry = view.RenderInPlace().ToArray();

        // Preview
        Model.AddElements(geometry);
        Model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void View_RenderInPlace_Wireframe_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(_geometry);
        View view = builder.Build();

        // Act
        GeometricElement[] geometry = view.RenderInPlace().ToArray();

        // Preview
        Model.AddElements(_geometry);
        Model.AddElements(geometry);
        Model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void View_RenderInPlace_Flat_Brickwork(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.25, .25, .25)
            .ViewDirection(viewDirection)
            .ElementsInView(_brickwork)
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        GeometricElement[] geometry = view.RenderInPlace().ToArray();

        // Preview
        Model.AddElements(geometry);
        Model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    [TestCase(ViewDirection.Bottom)]
    [TestCase(ViewDirection.Front)]
    [TestCase(ViewDirection.Left)]
    [TestCase(ViewDirection.Top)]
    [TestCase(ViewDirection.Right)]
    public void View_RenderInPlace_Flat_GeometricElements(ViewDirection viewDirection)
    {
        // Arrange
        ViewBuilder builder = new ViewBuilder()
            .ScopePadding(.050, .050, .050)
            .ViewDirection(viewDirection)
            .ElementsInView(_geometry)
            .RenderStrategy(new FlatRender());
        View view = builder.Build();

        // Act
        GeometricElement[] geometry = view.RenderInPlace().ToArray();

        // Preview
        Model.AddElements(_geometry);
        Model.AddElements(geometry);
        Model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }
}
