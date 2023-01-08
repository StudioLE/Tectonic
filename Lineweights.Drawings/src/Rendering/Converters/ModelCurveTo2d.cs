using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="ModelCurve"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ModelCurveTo2d<T> : IConverter<ModelCurve, Result<T>> where T : GeometricElement
{
    private readonly IRenderStrategy<T> _strategy;

    /// <inheritdoc cref="ModelCurveTo2d{T}"/>
    public ModelCurveTo2d(IRenderStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    /// <inheritdoc cref="ModelCurveTo2d{T}"/>
    public Result<T> Convert(ModelCurve modelCurve)
    {
        return _strategy.FromCurve(modelCurve.Curve, modelCurve.Transform, modelCurve.Material);
    }
}
