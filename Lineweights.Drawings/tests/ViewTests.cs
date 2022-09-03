using Lineweights.Drawings.Rendering;
using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Drawings.Tests;

[VisualizeAfterTest]
internal sealed class ViewTests
{
    private readonly Model _model = new();
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

    [SetUp]
    public void Setup()
    {
        _model.AddElements(CreateModelArrows.ByTransform(new()));
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
        _model.AddElements(geometry);
        _model.AddElements(view.Scope.Border);

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
        _model.AddElements(geometry);
        _model.AddElements(view.Scope.Border);

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
        _model.AddElements(geometry);
        _model.AddElements(view.Scope.Border);

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
        _model.AddElements(_geometry);
        _model.AddElements(geometry);
        _model.AddElements(view.Scope.Border);

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
        _model.AddElements(geometry);
        _model.AddElements(view.Scope.Border);

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
        _model.AddElements(_geometry);
        _model.AddElements(geometry);
        _model.AddElements(view.Scope.Border);

        // Assert
        Verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }
}
