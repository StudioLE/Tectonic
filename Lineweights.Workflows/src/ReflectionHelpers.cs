using System.Reflection;
using StudioLE.Core.Results;

namespace Lineweights.Workflows;

/// <summary>
/// Methods to help with reflection.
/// </summary>
internal static class ReflectionHelpers
{
    /// <summary>
    /// Try and get the value of the field.
    /// </summary>
    internal static IResult<T> TryGetFieldValue<T>(this object @this, string name, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        FieldInfo? property = flags is null
            ? type.GetField(name)
            : type.GetField(name, (BindingFlags)flags);
        if (property is null)
            return new Failure<T>($"Field {name} does not exist");
        object? value = property.GetValue(@this);
        return value is T tValue
            ? new Success<T>(tValue)
            : new Failure<T>($"Property type was {value?.GetType()}.");
    }
    /// <summary>
    /// Try and get the value of the property.
    /// </summary>
    internal static IResult<T> TryGetPropertyValue<T>(this object @this, string name, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        PropertyInfo? property = flags is null
            ? type.GetProperty(name)
            : type.GetProperty(name, (BindingFlags)flags);
        if (property is null)
            return new Failure<T>($"Property {name} does not exist");
        object? value = property.GetValue(@this);
        return value is T tValue
            ? new Success<T>(tValue)
            : new Failure<T>($"Property type was {value?.GetType()}.");
    }

    /// <summary>
    /// Create an instance.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    internal static T CreateInstanceAs<T>(Type type, params object[] args)
    {
        if (!typeof(T).IsAssignableFrom(type))
            throw new ArgumentException($"{nameof(T)} was not assignable from {type}.");
        object? instance = Activator.CreateInstance(type, args);
        return (T)instance;
    }
}
