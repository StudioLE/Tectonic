namespace Geometrician.Core.Distribution;

/// <summary>
/// Spacing around an <see cref="Element"/>.
/// </summary>
public struct Spacing
{
    /// <summary>
    /// The size of spacing in the x axis.
    /// </summary>
    public double X { get; set; } = 0;

    /// <summary>
    /// The size of spacing in the y axis.
    /// </summary>
    public double Y { get; set; } = 0;

    /// <summary>
    /// The size of spacing in the z axis.
    /// </summary>
    public double Z { get; set; } = 0;

    /// <inheritdoc cref="Spacing"/>
    public Spacing()
    {
    }

    /// <inheritdoc cref="Spacing"/>
    public Spacing(double spacing)
    {
        X = spacing;
        Y = spacing;
        Z = spacing;
    }

    /// <inheritdoc cref="Spacing"/>
    public Spacing(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
