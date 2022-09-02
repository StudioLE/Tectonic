using Ardalis.Result;

namespace Geometrician.Core.Forms;

public class NumberInputComponentBase : InputComponentBase<string>
{
    /// <summary>
    /// Get the value as the appropriate type.
    /// </summary>
    protected override string GetValue()
    {
        if (PropertyType == typeof(int))
            return PropertyValue?.ToString() ?? string.Empty;
        if (PropertyType == typeof(double))
            return PropertyValue?.ToString() ?? string.Empty;
        throw new InvalidOperationException("The property type was not supported.");
    }

    /// <summary>
    /// Set the value via the parent callback.
    /// </summary>
    protected override void SetValue(string valueAsString)
    {
        Result<object?> result;
        if (PropertyType == typeof(int))
            result = int.TryParse(valueAsString, out int value)
                ? value
                : Result<object?>.Error("was not an integer.");
        else if (PropertyType == typeof(double))
            result = double.TryParse(valueAsString, out double value)
                ? value
                : Result<object?>.Error("was not a number.");
        else
            result = Result<object?>.Error("The property type was not supported.");
        ParentCallback?.Invoke(result);
    }
}
