namespace Geometrician.Core.Elements.Comparers;

/// <summary>
/// Compare the equality of <see cref="Transform"/> by their <see cref="Matrix"/>.
/// </summary>
public sealed class TransformComparer : IEqualityComparer<Transform>
{
    /// <inheritdoc cref="TransformComparer"/>
    public bool Equals(Transform first, Transform second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first.GetType() != second.GetType())
            return false;

        return first.Matrix.Equals(second.Matrix);
    }

    /// <inheritdoc cref="TransformComparer"/>
    public int GetHashCode(Transform obj)
    {
        return HashCode.Combine(obj.Origin,
            obj.XAxis,
            obj.YAxis,
            obj.ZAxis,
            obj.Matrix);
    }
}
