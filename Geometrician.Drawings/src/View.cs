using Geometrician.Core.Serialization;
using Geometrician.Drawings.Rendering;
using Newtonsoft.Json;

namespace Geometrician.Drawings;

/// <summary>
/// A view which can be rendered and p on a <see cref="Sheet"/>.
/// <see cref="View"/> are constructed by the <see cref="ViewBuilder"/>.
/// </summary>
public sealed class View : Canvas
{
    /// <summary>
    /// The scope of the view.
    /// </summary>
    public ViewScope Scope { get; set; } = new();

    /// <summary>
    /// The scale the view is rendered at.
    /// </summary>
    public double Scale { get; set; }

    /// <summary>
    /// Should the border be rendered?
    /// </summary>
    public bool BorderVisible { get; set; } = true;

    /// <summary>
    /// The <see cref="IRenderStrategy"/> used to render the view.
    /// </summary>
    [JsonConverter(typeof(AbstractConverter))]
    public IRenderStrategy RenderStrategy { get; set; } = new WireframeRender();

    /// <inheritdoc cref="View"/>
    internal View()
    {
    }

    /// <summary>
    /// Render the <see cref="View"/> in place on the <see cref="ViewScope.Plane"/>.
    /// </summary>
    public ParallelQuery<GeometricElement> RenderInPlace()
    {
        ParallelQuery<GeometricElement> render = RenderStrategy.Render(Scope);

        return BorderVisible
            ? render.Concat(new GeometricElement[] { Scope.Border }.AsParallel())
            : render;
    }

    /// <summary>
    /// Render the <see cref="View"/> on the XY plane within the <see cref="Canvas"/>.
    /// </summary>
    public ParallelQuery<GeometricElement> Render()
    {
        ParallelQuery<GeometricElement> elementsInPlace = RenderInPlace();
        Transform translation = new(Scope.Origin.Negate());
        Transform rotation = TransformHelpers.RotationBetween(Scope.Orientation);
        return elementsInPlace
            .Select(element =>
            {
                element.Transform = element.Transform
                    .Concatenated(translation)
                    .Concatenated(rotation)
                    .Scaled(Scale);
                return element;
            });
    }
}
