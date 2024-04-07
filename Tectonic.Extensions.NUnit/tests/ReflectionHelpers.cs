using System.Reflection;
using StudioLE.Results;

namespace Tectonic.Extensions.NUnit.Tests;

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
    /// Try and set the value of the property.
    /// </summary>
    internal static IResult TrySetPropertyValue<T>(this object @this, string name, T value, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        PropertyInfo? property = flags is null
            ? type.GetProperty(name)
            : type.GetProperty(name, (BindingFlags)flags);
        if (property is null)
            return new Failure($"Property {name} does not exist");
        if (property.PropertyType != typeof(T))
            return new Failure($"Property type was {property.PropertyType}.");
        if (!property.CanWrite)
            return new Failure("Property has no setter.");
        property.SetValue(@this, value);
        return new Success();
    }

    /// <summary>
    /// Try and set the value of the property.
    /// </summary>
    internal static IResult<object> TryInvokeMethod(this object @this, string name, object[] parameters, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        MethodInfo? method = flags is null
            ? type.GetMethod(name)
            : type.GetMethod(name, (BindingFlags)flags);
        if (method is null)
            return new Failure<object>("Method does not exist");
        object? output = method.Invoke(@this, parameters);
        return new Success<object>(output!);
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
        return (T)instance!;
    }
}
