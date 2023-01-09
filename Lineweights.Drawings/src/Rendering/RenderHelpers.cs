using Lineweights.Drawings.Rendering.Converters;
using StudioLE.Core.Results;

namespace Lineweights.Drawings.Rendering;

/// <summary>
/// Methods to help with <see cref="IRenderStrategy"/>.
/// </summary>
internal static class RenderHelpers
{


    /// <inheritdoc cref="IRenderStrategy"/>
    internal static ParallelQuery<T> RenderAs<T>(this IRenderStrategy<T> @this, ViewScope viewScope) where T : GeometricElement
    {
        ElementTo2d<T> converter = new(@this, viewScope.Plane);
        return viewScope
            .Elements
            .AsParallel()
            .AsOrdered()
            .SelectMany(converter.Convert)
            .Where(x => x is Success<T>)
            .Select(x => ((Success<T>)x).Value);
        // TODO: Remove overlapping lines
        //.Distinct(new ModelCurveComparer());
    }

    internal static IEnumerable<Polygon> GetFacesAsPolygons(SolidOperation solid)
    {
        // TODO: Handle LocalTransform
        if (solid.LocalTransform is not null)
            throw new("Failed to get faces of a solid. There was an unhandled LocalTransform.");
        return solid
            .Solid
            .Faces
            .Select(kvp =>
            {
                Face face = kvp.Value;
                Vector3[] vertices = face
                    .Outer
                    .Edges
                    .Select(x => x.Vertex.Point)
                    .ToArray();
                return new Polygon(vertices);
            });

    }

    internal static IEnumerable<Polygon> GetTrianglesAsPolygons(Mesh mesh)
    {
        return mesh
            .Triangles
            .Select(triangle =>
            {
                Vector3[] vertices = triangle
                    .Vertices
                    .Select(x => x.Position)
                    .ToArray();
                return new Polygon(vertices);
            });
    }
}
