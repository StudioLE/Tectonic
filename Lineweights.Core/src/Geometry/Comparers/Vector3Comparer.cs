using StudioLE.Core.System;

namespace Lineweights.Core.Geometry.Comparers;

/// <summary>
/// Compare the equality of <see cref="Vector3"/> by their properties.
/// </summary>
public sealed class Vector3Comparer : IEqualityComparer<Vector3>
{
    /// <inheritdoc cref="Vector3"/>
    public bool Equals(Vector3 first, Vector3 second)
    {
        return first.X.ApproximatelyEquals(second.X)
               && first.Y.ApproximatelyEquals(second.Y)
               && first.Z.ApproximatelyEquals(second.Z);
    }

    /// <inheritdoc cref="Vector3"/>
    public int GetHashCode(Vector3 obj)
    {
        return HashCode.Combine(
            obj.X.Round(5),
            obj.Y.Round(5),
            obj.Z.Round(5));
    }
}
