using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert an <see cref="Element"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ElementTo2d<T> : IConverter<Element, IEnumerable<Result<T>>> where T : GeometricElement
{
    private readonly RenderBase<T> _strategy;

    /// <inheritdoc cref="ElementTo2d{T}"/>
    public ElementTo2d(RenderBase<T> strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="ElementTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(Element element)
    {
        return element switch
        {
            ModelCurve curve => new[] { new ModelCurveTo2d<T>(_strategy).Convert(curve) },
            ModelArrows arrows => new ModelArrowsTo2d<T>(_strategy).Convert(arrows),
            MeshElement mesh => new MeshElementTo2d<T>(_strategy).Convert(mesh),
            GeometricElement geometric => new GeometricElementTo2d<T>(_strategy).Convert(geometric),
            ElementInstance instance => new ElementInstanceTo2d<T>(_strategy).Convert(instance),
            // TODO: Add ModelTextToWireframe
            _ => new[] { Result<T>.Error($"Failed to convert element to 2d. Unsupported type ({element.GetType()}).") }
        };
    }
}
