using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="MeshElement"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class MeshElementTo2d<T> : IConverter<MeshElement, IEnumerable<Result<T>>> where T : GeometricElement
{
    private readonly RenderBase<T> _strategy;

    /// <inheritdoc cref="MeshElementTo2d{T}"/>
    public MeshElementTo2d(RenderBase<T> strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="MeshElementTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(MeshElement element)
    {
        return RenderHelpers.GetTrianglesAsPolygons(element.Mesh)
            .Select(polygon => _strategy.FromCurve(polygon, element.Transform, element.Material));
    }
}
