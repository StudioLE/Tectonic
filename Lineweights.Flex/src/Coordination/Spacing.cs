namespace Lineweights.Flex.Coordination;

/// <summary>
/// Spacing around an <see cref="Element"/>.
/// </summary>
public struct Spacing
{
    /// <summary>
    /// The size of spacing in the x axis.
    /// </summary>
    public double X { get; init; }

    /// <summary>
    /// The size of spacing in the y axis.
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    /// The size of spacing in the z axis.
    /// </summary>
    public double Z { get; init; }

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
