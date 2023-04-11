namespace Geometrician.Drawings.Rendering;

/// <summary>
/// The minimum and maximum distanced from a 3d element to its 2d rendered representation.
/// </summary>
public struct ZIndex
{
    /// <summary>
    /// The minimum distance from a 3d element to its 2d rendered representation.
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// The maximum distance from a 3d element to its 2d rendered representation.
    /// </summary>
    public double Max { get; }

    /// <inheritdoc cref="ZIndex"/>
    public ZIndex(double min, double max)
    {
        Min = min;
        Max = max;
    }

    /// <inheritdoc cref="ZIndex"/>
    public ZIndex(double[] zIndices)
    {
        Min = zIndices.Min();
        Max = zIndices.Max();
    }
}
