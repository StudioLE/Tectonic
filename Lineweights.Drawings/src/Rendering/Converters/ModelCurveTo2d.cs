using Ardalis.Result;
using StudioLE.Core.Conversion;

namespace Lineweights.Drawings.Rendering.Converters;

/// <summary>
/// Convert a <see cref="ModelCurve"/> to a 2d representation of type <typeparamref name="T"/>.
/// </summary>
internal sealed class ModelCurveTo2d<T> : IConverter<ModelCurve, Result<T>, RenderBase<T>> where T : GeometricElement
{
    /// <inheritdoc cref="ModelCurveTo2d{T}"/>
    public Result<T> Convert(ModelCurve modelCurve, RenderBase<T> render)
    {
        return render.FromCurve(modelCurve.Curve, modelCurve.Transform, modelCurve.Material);
    }
}
