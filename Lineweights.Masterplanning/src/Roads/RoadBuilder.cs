using Lineweights.Curves;
using Lineweights.Masterplanning.Distribution;
using Lineweights.Masterplanning.Elements;

namespace Lineweights.Masterplanning.Roads;

/// <summary>
/// Build a <see cref="Path"/>.
/// </summary>
public sealed class RoadBuilder
{
    /// <inheritdoc cref="Path.CenterLine"/>
    public IReadOnlyCollection<Spline> CenterSplines { get; set; } = Array.Empty<Spline>();

    /// <inheritdoc cref="PathBuilder"/>
    public PathBuilder CarriagewayBuilder { get; init; } = new()
    {
        Name = "Carriageway",
        Width = PathBuilder.DefaultWidth,
        Thickness = PathBuilder.DefaultThickness,
        Material = MaterialByName("Yellow")
    };

    /// <inheritdoc cref="PathBuilder"/>
    public PathBuilder FootwayBuilder { get; init; } = new()
    {
        Name = "Footway",
        AxialCreation = new BothOffsetAxialCreation
        {
            OffsetDistance = PathBuilder.DefaultWidth / 2
                             + 0.025
                             + PathBuilder.DefaultWidth / 8
        },
        Width = PathBuilder.DefaultWidth / 4,
        Thickness = PathBuilder.DefaultThickness,
        Material = MaterialByName("Aqua")
    };

    /// <inheritdoc cref="RoadBuilder"/>
    public IReadOnlyCollection<Path> Build()
    {
        return CenterSplines
            .SelectMany(x => Enumerable
                .Empty<Path>()
                .Concat(BuildCarriageway(x))
                .Concat(BuildFootway(x)))
            .ToArray();
    }

    private IReadOnlyCollection<Path> BuildCarriageway(Spline centerSpline)
    {
        CarriagewayBuilder.CenterSpline = centerSpline;
        return CarriagewayBuilder.Build();
    }

    private IReadOnlyCollection<Path> BuildFootway(Spline centerSpline)
    {
        FootwayBuilder.CenterSpline = centerSpline;
        return FootwayBuilder.Build();
    }
}
