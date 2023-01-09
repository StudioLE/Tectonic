using StudioLE.Core.Results;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert an <see cref="Element"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ElementTo2d<T> : IConverter<Element, IEnumerable<IResult<T>>> where T : GeometricElement
{
    private readonly IRenderStrategy<T> _strategy;
    private readonly Plane _plane;

    /// <inheritdoc cref="ElementTo2d{T}"/>
    public ElementTo2d(IRenderStrategy<T> strategy, Plane plane)
    {
        _strategy = strategy;
        _plane = plane;
    }

    /// <inheritdoc cref="ElementTo2d{T}"/>
    public IEnumerable<IResult<T>> Convert(Element element)
    {
        return element switch
        {
            ModelCurve curve => new[] { new ModelCurveTo2d<T>(_strategy, _plane).Convert(curve) },
            ModelArrows arrows => new ModelArrowsTo2d<T>(_strategy, _plane).Convert(arrows),
            MeshElement mesh => new MeshElementTo2d<T>(_strategy, _plane).Convert(mesh),
            GeometricElement geometric => new GeometricElementTo2d<T>(_strategy, _plane).Convert(geometric),
            ElementInstance instance => new ElementInstanceTo2d<T>(_strategy, _plane).Convert(instance),
            // TODO: Add ModelTextToWireframe
            _ => new[] { new Failure<T>($"Failed to convert element to 2d. Unsupported type ({element.GetType()}).") }
        };
    }
}
