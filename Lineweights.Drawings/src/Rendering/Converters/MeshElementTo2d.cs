using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="MeshElement"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class MeshElementTo2d<T> : IConverter<MeshElement, IEnumerable<Result<T>>, RenderBase<T>> where T : GeometricElement
{
    /// <inheritdoc cref="MeshElementTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(MeshElement element, RenderBase<T> render)
    {
        return RenderHelpers.GetTrianglesAsPolygons(element.Mesh)
            .Select(polygon => render.FromCurve(polygon, element.Transform, element.Material));
    }
}
