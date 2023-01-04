namespace Lineweights.Core.Elements.Comparers;

/// <summary>
/// Compare the equality of <see cref="Element"/> by their Id.
/// </summary>
public sealed class ElementIdComparer : IEqualityComparer<Element>
{
    /// <inheritdoc cref="ElementIdComparer"/>
    public bool Equals(Element first, Element second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first.GetType() != second.GetType())
            return false;
        return first.Id.Equals(second.Id);
    }

    /// <inheritdoc cref="ElementIdComparer"/>
    public int GetHashCode(Element obj)
    {
        return obj.Id.GetHashCode();;
    }
}
