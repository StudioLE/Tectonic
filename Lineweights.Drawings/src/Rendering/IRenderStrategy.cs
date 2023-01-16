using StudioLE.Core.Results;

namespace Lineweights.Drawings.Rendering;

/// <summary>
/// Render a <see cref="ViewScope"/> in place on <see cref="ViewScope.Plane"/>.
/// </summary>
public interface IRenderStrategy
{
    /// <inheritdoc cref="IRenderStrategy"/>
    public ParallelQuery<GeometricElement> Render(ViewScope viewScope);
}


/// <summary>
/// Render a <see cref="ViewScope"/> in place on <see cref="ViewScope.Plane"/>.
/// </summary>
public interface IRenderStrategy<T> : IRenderStrategy where T : GeometricElement
{
    /// <summary>
    /// Render <paramref name="curve"/> as <typeparamref name="T"/> on <paramref name="plane"/>.
    /// </summary>
    public IResult<T> FromCurve(Plane plane, Curve curve, Transform transform, Material material);
}
