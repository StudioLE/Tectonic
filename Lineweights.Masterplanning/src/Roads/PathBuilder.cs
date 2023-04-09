using Lineweights.Curves;
using Lineweights.Masterplanning.Distribution;
using Lineweights.Masterplanning.Elements;

namespace Lineweights.Masterplanning.Roads;

/// <summary>
/// Build a <see cref="Path"/>.
/// </summary>
public sealed class PathBuilder
{
    /// <summary>
    /// The default <see cref="Path.Width"/>.
    /// </summary>
    public const double DefaultWidth = 0.1;

    /// <summary>
    /// The default <see cref="Path"/> thickness.
    /// </summary>
    public const double DefaultThickness = 0.005;

    /// <inheritdoc cref="Element.Name"/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc cref="Path.CenterLine"/>
    public Spline? CenterSpline { get; set; }

    /// <inheritdoc cref="Distribution.AxialCreation"/>
    internal AxialCreation AxialCreation { get; set; } = new();

    /// <inheritdoc cref="Path.Width"/>
    public double Width { get; set; } = DefaultWidth;

    /// <summary>
    /// The thickness of the <see cref="Path"/>.
    /// </summary>
    public double Thickness { get; set; } = DefaultThickness;

    /// <inheritdoc cref="GeometricElement.Material"/>
    public Material Material { get; set; } = BuiltInMaterials.Default;

    /// <inheritdoc cref="PathBuilder"/>
    public IReadOnlyCollection<Path> Build()
    {
        if (CenterSpline is null)
            throw new($"Failed to create path. {nameof(CenterSpline)} must be set.");

        IReadOnlyCollection<Spline> splines = AxialCreation.Create(CenterSpline);

        return splines
            .Select(spline => new Path(spline, Width, Thickness, null!, Material, Name))
            .ToArray();
    }
}
