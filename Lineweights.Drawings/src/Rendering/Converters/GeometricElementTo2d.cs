using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="GeometricElement"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class GeometricElementTo2d<T> : IConverter<GeometricElement, IEnumerable<Result<T>>> where T : GeometricElement
{
    private readonly IRenderStrategy<T> _strategy;

    /// <inheritdoc cref="GeometricElementTo2d{T}"/>
    public GeometricElementTo2d(IRenderStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="GeometricElementTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(GeometricElement element)
    {
        if (element.Representation is null)
            return new[] { Result<T>.Error("GeometricElement didn't have a representation") };
        return element
            .Representation
            .SolidOperations
            .SelectMany(RenderHelpers.GetFacesAsPolygons)
            .Select(polygon => _strategy.FromCurve(polygon, element.Transform, element.Material));
    }
}
