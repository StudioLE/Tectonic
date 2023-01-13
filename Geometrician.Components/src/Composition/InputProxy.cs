using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StudioLE.Core.Results;

namespace Geometrician.Components.Composition;

/// <summary>
/// A proxy for an input property on an <see cref="InputProxy"/> class.
/// </summary>
public class InputProxy
{
    private readonly object _inputPackInstance;
    private readonly PropertyInfo _property;
    private readonly ValidationContext _context;

    /// <summary>
    /// The label displayed above the input.
    /// </summary>
    public string Label => _property.Name;

    /// <summary>
    /// The helper text displayed below the input.
    /// </summary>
    public string HelperText => _property.Name;

    /// <summary>
    /// The type of the input property.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// <inheritdoc cref="InputProxy"/>
    /// </summary>
    /// <param name="inputPackInstance">The instance of the parent class.</param>
    /// <param name="property">The property.</param>
    public InputProxy(object inputPackInstance, PropertyInfo property)
    {
        _inputPackInstance = inputPackInstance;
        _property = property;
        _context = new(_inputPackInstance)
        {
            MemberName = _property.Name
        };
        Type = Nullable.GetUnderlyingType(_property.PropertyType) ?? _property.PropertyType;
    }


    /// <summary>
    /// Get the current value of the property.
    /// </summary>
    /// <returns></returns>
    public object? GetValue()
    {
        return _property.GetValue(_inputPackInstance);
    }

    /// <summary>
    /// Get the current value of the property as <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidCastException">Property value was not a <typeparamref name="T"/>.</exception>
    public T GetValueAs<T>()
    {
        object? value = GetValue();
        return value is T tValue
            ? tValue
            : throw new InvalidCastException($"Property value was not a {typeof(T)}");
    }

    /// <summary>
    /// Set the value of the property.
    /// The value is only set if it's different to the previous value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetValue(object? value)
    {
        object? previousValue = GetValue();
        bool? hasChanged = !value?.Equals(previousValue);
        // bool hasChanged = !EqualityComparer<object>.Default.Equals(value, PropertyValue);
        if (hasChanged == true)
            _property.SetValue(_inputPackInstance, value);
    }

    /// <summary>
    /// Validate the value of the property according to the <see cref="System.ComponentModel.DataAnnotations"/>
    /// using <see cref="Validator.TryValidateProperty"/>
    /// </summary>
    /// <returns><see cref="Success"/> if valid otherwise <see cref="Failure"/> with errors messages stored to <see cref="Failure.Errors"/>.</returns>
    public IResult Validate()
    {
        object? value = GetValue();
        List<ValidationResult> results = new();
        return Validator.TryValidateProperty(value, _context, results)
            ? new Success()
            : new Failure(results
                .Select(x => x.ErrorMessage)
                .OfType<string>()
                .ToArray());
    }
}
