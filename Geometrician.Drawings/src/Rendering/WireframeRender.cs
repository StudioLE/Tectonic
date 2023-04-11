using StudioLE.Core.Results;

namespace Geometrician.Drawings.Rendering;

/// <inheritdoc/>
public sealed class WireframeRender : IRenderStrategy<ModelCurve>
{
    /// <inheritdoc/>
    public ParallelQuery<GeometricElement> Render(ViewScope viewScope)
    {
        return this.RenderAs(viewScope)
            .Select(x => x as GeometricElement);
    }

    /// <inheritdoc/>
    public IResult<ModelCurve> FromCurve(Plane plane, Curve curve, Transform transform, Material material)
    {
        Curve transformed = curve.Transformed(transform);
        IResult<Curve> result = transformed.TryProject(plane);
        return result is Success<Curve> success
            ? new Success<ModelCurve>(new(success, material))
            : new Failure<ModelCurve>(result.Errors);
    }
}
