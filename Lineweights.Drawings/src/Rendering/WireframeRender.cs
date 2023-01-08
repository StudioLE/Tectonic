using Ardalis.Result;

namespace Lineweights.Drawings.Rendering;

/// <inheritdoc/>
public sealed class WireframeRender : IRenderStrategy<ModelCurve>
{
     /// <inheritdoc cref="ViewScope.Plane"/>
     public Plane Plane { get; private set; }

     /// <inheritdoc />
    public ParallelQuery<GeometricElement> Render(ViewScope viewScope)
    {
        Plane = viewScope.Plane;
        return this.RenderAs(viewScope)
            .Select(x => x as GeometricElement);
    }

    /// <inheritdoc />
    public Result<ModelCurve> FromCurve(Curve curve, Transform transform, Material material)
    {
        Curve transformed = curve.Transformed(transform);
        Result<Curve> result = transformed.TryProject(Plane);
        return result.IsSuccess
            ? new(new(result, material))
            : Result<ModelCurve>.Error(result.Errors.ToArray());
    }
}
