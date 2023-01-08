using Ardalis.Result;
using Lineweights.Drawings.Rendering.Converters;

namespace Lineweights.Drawings.Rendering;

/// <summary>
/// Render a <see cref="ViewScope"/> in place on <see cref="ViewScope.Plane"/>
/// as 2d representation of type <typeparamref name="T"/>.
/// </summary>
public abstract class RenderBase<T> : IRenderStrategy where T : GeometricElement
{
    /// <inheritdoc cref="ViewScope.Plane"/>
    public Plane Plane { get; private set; } = null!;

    /// <inheritdoc cref="IRenderStrategy"/>
    public ParallelQuery<T> RenderAsT(ViewScope viewScope)
    {
        Plane = viewScope.Plane;
        ElementTo2d<T> converter = new(this);
        return viewScope
            .Elements
            .AsParallel()
            .AsOrdered()
            .SelectMany(converter.Convert)
            .Where(x => x.IsSuccess)
            .Select(x => x.Value);
        // TODO: Remove overlapping lines
        //.Distinct(new ModelCurveComparer());
    }

    /// <inheritdoc cref="IRenderStrategy"/>
    public abstract ParallelQuery<GeometricElement> Render(ViewScope viewScope);

    /// <summary>
    /// Render <paramref name="curve"/> as <typeparamref name="T"/> on <see cref="Plane"/>.
    /// </summary>
    public abstract Result<T> FromCurve(Curve curve, Transform transform, Material material);
}
