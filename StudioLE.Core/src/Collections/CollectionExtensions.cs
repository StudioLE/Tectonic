namespace StudioLE.Core.Collections;

/// <summary>
/// Methods to help with <see cref="IReadOnlyCollection{T}"/>.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Determine if the collection has an element at <paramref name="index"/>.
    /// </summary>
    public static bool HasIndex<T>(this IReadOnlyCollection<T> @this, int index)
    {
        return index >= 0 && index < @this.Count;
    }

    /// <summary>
    /// Try and get the element at <paramref name="index"/>.
    /// </summary>
    public static bool TryGetAt<T>(this IReadOnlyCollection<T> @this, int index, out T? element)
    {
        if (@this.HasIndex(index))
        {
            element = @this.ElementAt(index);
            return true;
        }
        element = default;
        return false;
    }

    /// <summary>
    /// Get element at <paramref name="index"/>.
    /// If <paramref name="index"/> exceeds the collection count then search again from the begin.
    /// </summary>
    public static T ElementAtWithWrapping<T>(this IReadOnlyCollection<T> collection, int index)
    {
        if (collection.Count == 0)
            throw new ArgumentException($"{nameof(ElementAtWithWrapping)} failed. The collection was empty.", nameof(collection));
        int wrappedIndex = index % collection.Count;
        return collection.ElementAt(wrappedIndex);
    }
}
