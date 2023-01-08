using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="ModelArrows"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ModelArrowsTo2d<T> : IConverter<ModelArrows, IEnumerable<Result<T>>> where T : GeometricElement
{
    private readonly RenderBase<T> _strategy;

    /// <inheritdoc cref="ModelArrowsTo2d{T}"/>
    public ModelArrowsTo2d(RenderBase<T> strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="ModelArrowsTo2d{T}"/>
    public IEnumerable<Result<T>> Convert(ModelArrows arrows)
    {
        return arrows
            .Vectors
            .Select(x =>
            {
                // TODO: Convert the arrow heads as well
                Line line = new(x.origin, x.direction, x.magnitude);
                Material material = x.color is null
                    ? arrows.Material
                    : new(x.color.ToString(), (Color)x.color);
                return _strategy.FromCurve(line, arrows.Transform, material);
            });
    }
}
