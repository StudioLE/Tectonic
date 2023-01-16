namespace Lineweights.Core.Elements.Comparers;

/// <summary>
/// Compare the <typeparamref name="TValue"/>.
/// </summary>
public sealed class KeyValuePairValueComparer<TKey, TValue> : IEqualityComparer<KeyValuePair<TKey, TValue>>
{
    /// <inheritdoc cref="ElementIdComparer"/>
    public bool Equals(KeyValuePair<TKey, TValue> first, KeyValuePair<TKey, TValue> second)
    {
        if (first.GetType() != second.GetType())
            return false;
        return first.Value?.Equals(second.Value) ?? false;
    }

    /// <inheritdoc cref="ElementIdComparer"/>
    public int GetHashCode(KeyValuePair<TKey, TValue> obj)
    {
        return obj.Value?.GetHashCode() ?? obj.GetHashCode();
    }
}
