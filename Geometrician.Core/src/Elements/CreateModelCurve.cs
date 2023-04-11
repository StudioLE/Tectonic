namespace Geometrician.Core.Elements;

/// <summary>
/// Methods to create <see cref="ModelCurve"/>.
/// </summary>
public static class CreateModelCurve
{
    /// <summary>
    /// Create multiple <see cref="ModelCurve"/> with sequentially alternating materials.
    /// </summary>
    public static IEnumerable<ModelCurve> WithAlternatingMaterials(
        IEnumerable<Curve> curves,
        Material firstMaterial,
        Material secondMaterial)
    {
        return curves
            .Select((curve, i) => new ModelCurve(curve,
                i % 2 == 0
                    ? firstMaterial
                    : secondMaterial));
    }

    /// <summary>
    /// Create multiple <see cref="ModelCurve"/> with sequentially alternating materials.
    /// </summary>
    public static IEnumerable<ModelCurve> WithAlternatingMaterials(
        IReadOnlyCollection<Vector3> vertices,
        Material firstMaterial,
        Material secondMaterial)
    {
        return vertices
            .Take(vertices.Count - 1)
            .Zip(vertices.Skip(1),
                (start, end) => new Line(start, end))
            .Select((line, i) => new ModelCurve(line,
                i % 2 == 0
                    ? firstMaterial
                    : secondMaterial));
    }

    /// <summary>
    /// Create multiple <see cref="ModelCurve"/> with sequentially alternating materials.
    /// </summary>
    public static IEnumerable<ModelCurve> WithAlternatingMaterials(
        IEnumerable<Curve> curves,
        string firstColor,
        string secondColor)
    {
        return WithAlternatingMaterials(curves, MaterialByName(firstColor), MaterialByName(secondColor));
    }

    /// <summary>
    /// Create multiple <see cref="ModelCurve"/> with sequentially alternating materials.
    /// </summary>
    public static IEnumerable<ModelCurve> WithAlternatingMaterials(
        IReadOnlyCollection<Vector3> vertices,
        string firstColor,
        string secondColor)
    {
        return WithAlternatingMaterials(vertices, MaterialByName(firstColor), MaterialByName(secondColor));
    }
}
