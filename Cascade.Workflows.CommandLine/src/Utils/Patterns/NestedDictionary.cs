namespace Cascade.Workflows.CommandLine.Utils.Patterns;

public class NestedDictionary<TKey, TValue> where TValue : class where TKey : notnull
{
    private readonly Dictionary<TKey, NestedDictionary<TKey, TValue>> _children = new();

    public TValue? Value { get; private set; }

    private bool ContainsKey(TKey key)
    {
        return _children.ContainsKey(key);
    }

    private NestedDictionary<TKey, TValue>? GetByKey(TKey key)
    {
        return _children.GetValueOrDefault(key);
    }

    public NestedDictionary<TKey, TValue>? GetByKeys(IReadOnlyCollection<TKey> keys)
    {
        if (!keys.Any())
            return null;
        NestedDictionary<TKey, TValue>? current = this;
        foreach (TKey key in keys)
        {
            current = current.GetByKey(key);
            if (current is null)
                return null;
        }
        return current;
    }

    public NestedDictionary<TKey, TValue> GetOrCreateByKeys(IReadOnlyCollection<TKey> keys)
    {
        if (!keys.Any())
            throw new("keys can't be empty.");
        NestedDictionary<TKey, TValue> current = this;
        foreach (TKey key in keys)
        {
            if (!current.ContainsKey(key))
                current._children.Add(key, new());
            current = current.GetByKey(key) ?? throw new("Unexpected null.");
        }
        return current;
    }

    public void SetByKeys(IReadOnlyCollection<TKey> keys, TValue value)
    {
        NestedDictionary<TKey, TValue> dictionary = GetOrCreateByKeys(keys);
        dictionary.Value = value;
    }

}
