using Ardalis.Result;

namespace Lineweights.Drawings.Rendering;

/// <inheritdoc/>
public sealed class WireframeRender : IRenderStrategy<ModelCurve>
{
    /// <inheritdoc />
    public ParallelQuery<GeometricElement> Render(ViewScope viewScope)
    {
        return this.RenderAs(viewScope)
            .Select(x => x as GeometricElement);
    }

    /// <inheritdoc />
    public Result<ModelCurve> FromCurve(Plane plane, Curve curve, Transform transform, Material material)
    {
        Curve transformed = curve.Transformed(transform);
        Result<Curve> result = transformed.TryProject(plane);
        return result.IsSuccess
            ? new(new(result, material))
            : Result<ModelCurve>.Error(result.Errors.ToArray());
    }
}
