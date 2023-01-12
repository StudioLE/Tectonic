using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StudioLE.Core.Results;

namespace Geometrician.Core.Composition;

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
