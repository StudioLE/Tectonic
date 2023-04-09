using Lineweights.Curves;
using Lineweights.Masterplanning.Elements;
using Lineweights.Masterplanning.Roads;

namespace Lineweights.Masterplanning;

/// <summary>
/// Build a masterplan of major and minor roads.
/// </summary>
public sealed class MasterplanBuilder
{
    /// <summary>
    /// The major road center lines.
    /// </summary>
    public IReadOnlyCollection<Spline> MajorSplines { get; set; } = Array.Empty<Spline>();

    /// <summary>
    /// The minor road center lines.
    /// </summary>
    public IReadOnlyCollection<Spline> MinorSplines { get; set; } = Array.Empty<Spline>();

    /// <summary>
    /// The builder for the major roads.
    /// </summary>
    public RoadBuilder MajorRoadBuilder { get; set; } = new();

    /// <summary>
    /// The builder for the minor roads.
    /// </summary>
    public RoadBuilder MinorRoadBuilder { get; set; } = new();

    /// <inheritdoc cref="MasterplanBuilder"/>
    public IReadOnlyCollection<Path> Build()
    {
        return Enumerable
            .Empty<Path>()
            .Concat(BuildMajorRoads())
            .Concat(BuildMinorRoads())
            .ToArray();
    }

    private IReadOnlyCollection<Path> BuildMajorRoads()
    {
        MajorRoadBuilder.CenterSplines = MajorSplines;
        return MajorRoadBuilder.Build();
    }

    private IReadOnlyCollection<Path> BuildMinorRoads()
    {
        MinorRoadBuilder.CenterSplines = MinorSplines;
        return MinorRoadBuilder.Build();
    }
}
