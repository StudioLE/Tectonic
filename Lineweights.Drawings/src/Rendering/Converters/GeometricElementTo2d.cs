using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="GeometricElement"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class GeometricElementTo2d<T> : IConverter<GeometricElement, IEnumerable<IResult<T>>> where T : GeometricElement
{
    private readonly IRenderStrategy<T> _strategy;
    private readonly Plane _plane;

    /// <inheritdoc cref="GeometricElementTo2d{T}"/>
    public GeometricElementTo2d(IRenderStrategy<T> strategy, Plane plane)
    {
        _strategy = strategy;
        _plane = plane;
    }

    /// <inheritdoc cref="GeometricElementTo2d{T}"/>
    public IEnumerable<IResult<T>> Convert(GeometricElement element)
    {
        if (element.Representation is null)
            return new[] { new Failure<T>("GeometricElement didn't have a representation") };
        return element
            .Representation
            .SolidOperations
            .SelectMany(RenderHelpers.GetFacesAsPolygons)
            .Select(polygon => _strategy.FromCurve(_plane, polygon, element.Transform, element.Material));
    }
}
