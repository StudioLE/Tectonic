using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert an <see cref="ElementInstance"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ElementInstanceTo2d<T> : IConverter<ElementInstance, IEnumerable<Result<T>>> where T : GeometricElement
{
    private readonly RenderBase<T> _strategy;

    /// <inheritdoc cref="ElementInstanceTo2d{T}"/>
    public ElementInstanceTo2d(RenderBase<T> strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="ElementInstanceTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(ElementInstance instance)
    {
        if (instance.BaseDefinition.Representation is null)
            return new[] { Result<T>.Error("BaseDefinition didn't have a representation") };
        Transform transform = instance
            .Transform
            .Concatenated(instance.BaseDefinition.Transform);
        return instance
            .BaseDefinition
            .Representation
            .SolidOperations
            .SelectMany(RenderHelpers.GetFacesAsPolygons)
            .Select(polygon => _strategy.FromCurve(polygon, transform, instance.BaseDefinition.Material));
    }
}
