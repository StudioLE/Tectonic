using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert an <see cref="Element"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ElementTo2d<T> : IConverter<Element, IEnumerable<Result<T>>, RenderBase<T>> where T : GeometricElement
{
    /// <inheritdoc cref="ElementTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(Element element, RenderBase<T> render)
    {
        return element switch
        {
            ModelCurve curve => new[] { new ModelCurveTo2d<T>().Convert(curve, render) },
            ModelArrows arrows => new ModelArrowsTo2d<T>().Convert(arrows, render),
            MeshElement mesh => new MeshElementTo2d<T>().Convert(mesh, render),
            GeometricElement geometric => new GeometricElementTo2d<T>().Convert(geometric, render),
            ElementInstance instance => new ElementInstanceTo2d<T>().Convert(instance, render),
            // TODO: Add ModelTextToWireframe
            _ => new[] { Result<T>.Error($"Failed to convert element to 2d. Unsupported type ({element.GetType()}).") }
        };
    }
}
