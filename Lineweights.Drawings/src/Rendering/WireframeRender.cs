using Ardalis.Result;

namespace Lineweights.Drawings.Rendering;

/// <inheritdoc/>
public sealed class WireframeRender : RenderBase<ModelCurve>
{
    /// <inheritdoc />
    public override ParallelQuery<GeometricElement> Render(ViewScope viewScope)
    {
        return RenderAsT(viewScope)
            .Select(x => x as GeometricElement);
    }

    /// <inheritdoc />
    public override Result<ModelCurve> FromCurve(Curve curve, Transform transform, Material material)
{
        Curve transformed = curve.Transformed(transform);
        Result<Curve> result = transformed.TryProject(Plane);
        return result.IsSuccess
            ? new(new(result, material))
            : Result<ModelCurve>.Error(result.Errors.ToArray());
    }
}
