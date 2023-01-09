using StudioLE.Core.Results;

namespace Lineweights.Drawings.Rendering;

/// <inheritdoc/>
public sealed class FlatRender : IRenderStrategy<Panel>
{
    /// <inheritdoc />
    public ParallelQuery<GeometricElement> Render(ViewScope viewScope)
    {
        // TODO: ZIndex order disabled as results are unreliable.
        return this.RenderAs(viewScope)
            //.OrderBy(panel =>
            //    {
            //        IResult<ZIndex> result = panel.GetProperty<ZIndex>();
            //        Validate.OrThrow(result, "Failed to render the view.");
            //        return result.Value.Min;
            //    })
            .Select(x => x as GeometricElement);
    }

    /// <inheritdoc />
    public IResult<Panel> FromCurve(Plane plane, Curve curve, Transform transform, Material material)
    {
        if (curve is not Polygon polygon)
            return new Failure<Panel>("Curve must be a polygon.");

        double[] zIndices = polygon
            .Vertices
            .Select(x =>
            {
                double zIndex = x.DistanceTo(plane);
                // TODO: A negative result means the element is outside or intersecting with the view.
                //if (zIndex < 0)
                //    throw new("Element is outside the view.");
                zIndex = Math.Abs(zIndex);
                return zIndex;
            })
            .ToArray();

        Polygon transformed = polygon.TransformedPolygon(transform);
        IResult<Polygon> result = transformed.TryProject(plane);
        if (result is not Success<Polygon> success)
            return new Failure<Panel>(result.Errors.ToArray());

        Polygon projected = success;
        Panel panel = new(projected, material);
        panel.SetProperty<ZIndex>(new(zIndices));
        return new Success<Panel>(panel);
    }
}
