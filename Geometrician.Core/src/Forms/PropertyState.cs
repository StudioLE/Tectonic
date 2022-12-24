using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ardalis.Result;

namespace Geometrician.Core.Forms;

public class PropertyState
{
    private readonly object _instance;

    private readonly ValidationContext _context;

    public PropertyInfo Property { get; }

    public PropertyState(object instance, PropertyInfo property)
    {
        _instance = instance;
        Property = property;
        _context = new(_instance)
        {
            MemberName = Property.Name
        };
    }

    public object? GetValue()
    {
        return Property.GetValue(_instance);
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
            Property.SetValue(_instance, value);
    }

    public Result<bool> Validate()
    {
        object? value = GetValue();
        List<ValidationResult> results = new();
        bool isValid = Validator.TryValidateProperty(value, _context, results);
        return isValid
            ? true
            : Result<bool>.Error(results.Select(x => x.ErrorMessage).ToArray());
    }
}
