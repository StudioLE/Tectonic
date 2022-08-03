using Ardalis.Result;

namespace Lineweights.Drawings.Rendering;

/// <inheritdoc/>
public sealed class FlatRender : RenderBase<Panel>
{
    /// <inheritdoc />
    public override ParallelQuery<GeometricElement> Render(ViewScope viewScope)
    {
        // TODO: ZIndex order disabled as results are unreliable.
        return RenderAsT(viewScope)
            //.OrderBy(panel =>
            //    {
            //        Result<ZIndex> result = panel.GetProperty<ZIndex>();
            //        Validate.OrThrow(result, "Failed to render the view.");
            //        return result.Value.Min;
            //    })
            .Select(x => x as GeometricElement);
    }

    /// <inheritdoc />
    public override Result<Panel> FromCurve(Curve curve, Transform transform, Material material)
    {
        if (curve is not Polygon polygon)
            return Result<Panel>.Error("Curve must be a polygon.");

        double[] zIndices = polygon
            .Vertices
            .Select(x =>
            {
                double zIndex = x.DistanceTo(Plane);
                // TODO: A negative result means the element is outside or intersecting with the view.
                //if (zIndex < 0)
                //    throw new("Element is outside the view.");
                zIndex = Math.Abs(zIndex);
                return zIndex;
            })
            .ToArray();

        Polygon transformed = polygon.TransformedPolygon(transform);
        Result<Polygon> result = transformed.TryProject(Plane);
        if (!result.IsSuccess)
            return Result<Panel>.Error(result.Errors.ToArray());

        Polygon projected = result;
        Panel panel = new(projected, material);
        panel.SetProperty<ZIndex>(new(zIndices));
        return panel;
    }
}
