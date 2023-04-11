namespace Geometrician.Core.Elements.Comparers;

/// <summary>
/// Compare the equality of <see cref="Element"/> by their Id.
/// </summary>
public sealed class ElementTypeComparer : IEqualityComparer<Element>
{
    /// <inheritdoc cref="ElementIdComparer"/>
    public bool Equals(Element first, Element second)
    {
        if (ReferenceEquals(first, second))
            return true;
        return first.GetType() == second.GetType();
    }

    /// <inheritdoc cref="ElementIdComparer"/>
    public int GetHashCode(Element obj)
    {
        return obj.GetType().GetHashCode();
    }
}
