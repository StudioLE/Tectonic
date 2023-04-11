using Geometrician.Curves;

namespace Geometrician.Masterplanning.Distribution;

// TODO: Finish AxialCreation or remove it.

/// <summary>
/// A strategy for creating <see cref="Spline"/> offset an axis.
/// </summary>
/// <remarks>
/// WARNING: <see cref="AxialCreation"/> is an experimental work in progress.
/// </remarks>
internal class AxialCreation
{
    /// <summary>
    /// The distance to offset the spline.
    /// </summary>
    public double OffsetDistance { get; set; }

    /// <inheritdoc cref="AxialCreation"/>
    public virtual IReadOnlyCollection<Spline> Create(Spline centerSpline)
    {
        return new[]
        {
            centerSpline
        };
    }
}

/// <inheritdoc/>
internal sealed class BothOffsetAxialCreation : AxialCreation
{
    /// <inheritdoc/>
    public override IReadOnlyCollection<Spline> Create(Spline centerSpline)
    {
        return new[]
        {
            centerSpline.Offset(OffsetDistance, true),
            centerSpline.Offset(OffsetDistance, false)
        };
    }
}

/// <inheritdoc/>
internal sealed class NearOffsetAxialCreation : AxialCreation
{
    /// <inheritdoc/>
    public override IReadOnlyCollection<Spline> Create(Spline centerSpline)
    {
        return new[]
        {
            centerSpline.Offset(OffsetDistance, true)
        };
    }
}

/// <inheritdoc/>
internal sealed class FarOffsetAxialCreation : AxialCreation
{
    /// <inheritdoc/>
    public override IReadOnlyCollection<Spline> Create(Spline centerSpline)
    {
        return new[]
        {
            centerSpline.Offset(OffsetDistance, false)
        };
    }
}
