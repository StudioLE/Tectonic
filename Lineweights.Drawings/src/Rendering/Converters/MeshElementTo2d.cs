using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="MeshElement"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class MeshElementTo2d<T> : IConverter<MeshElement, IEnumerable<IResult<T>>> where T : GeometricElement
{
    private readonly IRenderStrategy<T> _strategy;
    private readonly Plane _plane;

    /// <inheritdoc cref="MeshElementTo2d{T}"/>
    public MeshElementTo2d(IRenderStrategy<T> strategy, Plane plane)
    {
        _strategy = strategy;
        _plane = plane;
    }

    /// <inheritdoc cref="MeshElementTo2d{T}"/>
    public IEnumerable<IResult<T>> Convert(MeshElement element)
    {
        return RenderHelpers.GetTrianglesAsPolygons(element.Mesh)
            .Select(polygon => _strategy.FromCurve(_plane, polygon, element.Transform, element.Material));
    }
}
