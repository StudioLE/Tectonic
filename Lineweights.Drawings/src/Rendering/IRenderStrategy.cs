using Ardalis.Result;

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
    /// <inheritdoc cref="ViewScope.Plane"/>
    public Plane Plane { get; }

     /// <summary>
     /// Render <paramref name="curve"/> as <typeparamref name="T"/> on <see cref="Plane"/>.
     /// </summary>
     public Result<T> FromCurve(Curve curve, Transform transform, Material material);
}
