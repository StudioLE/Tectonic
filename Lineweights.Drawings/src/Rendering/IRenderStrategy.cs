namespace Lineweights.Drawings.Rendering;

/// <summary>
/// Render a <see cref="ViewScope"/> in place on <see cref="ViewScope.Plane"/>.
/// </summary>
public interface IRenderStrategy
{
    /// <inheritdoc cref="IRenderStrategy"/>
    public ParallelQuery<GeometricElement> Render(ViewScope viewScope);
}
