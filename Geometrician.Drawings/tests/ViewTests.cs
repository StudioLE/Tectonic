using Geometrician.Diagnostics;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Diagnostics.Samples;
using Geometrician.Drawings.Rendering;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Drawings.Tests;

internal sealed class ViewTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private readonly IReadOnlyCollection<ElementInstance> _brickwork = Scenes.Brickwork();
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();
    private Model _model = new();

    [SetUp]
    public void Setup()
    {
        _model.AddElements(CreateModelArrows.ByTransform(new()));
    }

    [TestCase(ViewDirection.Back)]
    public async Task View_Render_Wireframe_Brickwork(ViewDirection viewDirection)
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
        await _verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    public async Task View_Render_Wireframe_GeometricElements(ViewDirection viewDirection)
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
        await _verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    public async Task View_RenderInPlace_Wireframe_Brickwork(ViewDirection viewDirection)
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
        await _verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    public async Task View_RenderInPlace_Wireframe_GeometricElements(ViewDirection viewDirection)
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
        await _verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    public async Task View_RenderInPlace_Flat_Brickwork(ViewDirection viewDirection)
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
        await _verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TestCase(ViewDirection.Back)]
    public async Task View_RenderInPlace_Flat_GeometricElements(ViewDirection viewDirection)
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
        await _verify.ModelCurvesByCurve(geometry.OfType<ModelCurve>());
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model);
        _model = new();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
