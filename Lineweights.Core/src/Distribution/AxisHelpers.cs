namespace Lineweights.Core.Distribution;

/// <summary>
/// Methods to help with axis.
/// </summary>
public static class AxisHelpers
{
    /// <summary>
    /// Get the size of <paramref name="vector"/> in the <paramref name="axis"/>.
    /// </summary>
    public static double Dimension(this Vector3 axis, Vector3 vector)
    {
        if (axis.IsParallelTo(Vector3.XAxis))
            return vector.X;
        if (axis.IsParallelTo(Vector3.YAxis))
            return vector.Y;
        if (axis.IsParallelTo(Vector3.ZAxis))
            return vector.Z;
        throw new("Failed to get dimension of vector in axis. Currently only X, Y, and Z axis are supported.");
    }

    /// <summary>
    /// Get the size of <paramref name="bounds"/> in the <paramref name="axis"/>.
    /// </summary>
    public static double Dimension(this Vector3 axis, BBox3 bounds)
    {
        if (axis.IsParallelTo(Vector3.XAxis))
            return bounds.Max.X - bounds.Min.X;
        if (axis.IsParallelTo(Vector3.YAxis))
            return bounds.Max.Y - bounds.Min.Y;
        if (axis.IsParallelTo(Vector3.ZAxis))
            return bounds.Max.Z - bounds.Min.Z;
        throw new("Failed to get dimension of bounds in axis. Currently only X, Y, and Z axis are supported.");
    }

    /// <summary>
    /// Get the size of <paramref name="spacing"/> in the <paramref name="axis"/>.
    /// </summary>
    public static double Dimension(this Vector3 axis, Spacing spacing)
    {
        if (axis.IsParallelTo(Vector3.XAxis))
            return spacing.X;
        if (axis.IsParallelTo(Vector3.YAxis))
            return spacing.Y;
        if (axis.IsParallelTo(Vector3.ZAxis))
            return spacing.Z;
        throw new("Failed to get dimension of spacing in axis. Currently only X, Y, and Z axis are supported.");
    }
}
