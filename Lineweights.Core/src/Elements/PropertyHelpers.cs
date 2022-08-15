using Ardalis.Result;

namespace Lineweights.Core.Elements;

/// <summary>
/// Methods to help with <see cref="Element.AdditionalProperties"/>.
/// </summary>
public static class PropertyHelpers
{
    /// <summary>
    /// Get the value of an additional property by <paramref name="key"/>.
    /// Returns true if successful.
    /// </summary>
    public static Result<T> GetProperty<T>(this Element element, string key)
    {
        bool isSet = element
            .AdditionalProperties
            .TryGetValue(key, out object obj);
        if (!isSet)
            return Result<T>.NotFound();
        if (obj is not T value)
            return Result<T>.Error("InvalidType");
        return value;
    }

    /// <summary>
    /// Set the <paramref name="value"/> of an additional property by <paramref name="key"/>.
    /// The key is the full name, including namespace of <typeparamref name="T"/>.
    /// Returns true if successful.
    /// </summary>
    public static void SetProperty<T>(this Element element, string key, T value)
    {
        element.AdditionalProperties[key] = value;
    }

    /// <summary>
    /// Get the value of an additional property.
    /// The full name (including namespace) of <typeparamref name="T"/> is used as the key.
    /// Returns true if successful.
    /// </summary>
    public static Result<T> GetProperty<T>(this Element element)
    {
        return element.GetProperty<T>(KeyByType<T>());
    }

    /// <summary>
    /// Set the <paramref name="value"/> of an additional property.
    /// The full name (including namespace) of <typeparamref name="T"/> is used as the key.
    /// </summary>
    public static void SetProperty<T>(this Element element, T value)
    {
        element.SetProperty(KeyByType<T>(), value);
    }

    /// <summary>
    /// Get the values of an additional property.
    /// The full name (including namespace) of <see cref="IEnumerable{T}"/> is used as the key.
    /// Returns true if successful.
    /// </summary>
    public static Result<IEnumerable<T>> TryGetProperties<T>(this Element element)
    {
        return element.GetProperty<IEnumerable<T>>(KeyByType<IEnumerable<T>>());
    }

    /// <summary>
    /// Set the <paramref name="values"/> of an additional property.
    /// The full name (including namespace) of <typeparamref name="T"/> is used as the key.
    /// </summary>
    public static void SetProperties<T>(this Element element, IEnumerable<T> values)
    {
        element.SetProperty(KeyByType<IEnumerable<T>>(), values);
    }

    private static string KeyByType<T>()
    {
        Type type = typeof(T);
        Type? underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is not null)
            type = underlyingType;
        return type.FullName!;
    }
}
