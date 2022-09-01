using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows;

/// <summary>
/// Methods to help with reflection.
/// </summary>
internal static class ReflectionHelpers
{
    /// <summary>
    /// Try and get the value of the field.
    /// </summary>
    internal static Result<T> TryGetFieldValue<T>(this object @this, string name, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        FieldInfo? property = flags is null
            ? type.GetField(name)
            : type.GetField(name, (BindingFlags)flags);
        if(property is null)
            return Result<T>.Error($"Field {name} does not exist");
        object? value = property.GetValue(@this);
        if (value is T tValue)
            return tValue;
        return Result<T>.Error($"Property type was {value?.GetType()}.");
    }
    /// <summary>
    /// Try and get the value of the property.
    /// </summary>
    internal static Result<T> TryGetPropertyValue<T>(this object @this, string name, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        PropertyInfo? property = flags is null
            ? type.GetProperty(name)
            : type.GetProperty(name, (BindingFlags)flags);
        if(property is null)
            return Result<T>.Error($"Property {name} does not exist");
        object? value = property.GetValue(@this);
        if (value is T tValue)
            return tValue;
        return Result<T>.Error($"Property type was {value?.GetType()}.");
    }
}
