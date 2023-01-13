using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StudioLE.Core.Results;

namespace Geometrician.Components.Composition;

public class InputProxy
{
    private readonly object _inputPackInstance;
    private readonly PropertyInfo _property;
    private readonly ValidationContext _context;

    public string Label => _property.Name;

    public string HelperText => _property.Name;

    public Type Type { get; }

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

    public object? GetValue()
    {
        return _property.GetValue(_inputPackInstance);
    }

    public T GetValueAs<T>()
    {
        object? value = GetValue();
        return value is T tValue
            ? tValue
            : throw new($"Property value was not a {typeof(T)}");
    }

    public void SetValue(object? value)
    {
        object? previousValue = GetValue();
        bool? hasChanged = !value?.Equals(previousValue);
        // bool hasChanged = !EqualityComparer<object>.Default.Equals(value, PropertyValue);
        if (hasChanged == true)
            _property.SetValue(_inputPackInstance, value);
    }

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
