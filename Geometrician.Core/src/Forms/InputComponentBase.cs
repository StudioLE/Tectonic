using Ardalis.Result;
using Microsoft.AspNetCore.Components;

namespace Geometrician.Core.Forms;

public class InputComponentBase<T> : ComponentBase
{
    /// <summary>
    /// The initial value of the property.
    /// </summary>
    /// <remarks>
    /// This is not bound therefore it is only updated when the parent state changes.
    /// </remarks>
    [Parameter]
    public object? PropertyValue { get; set; }

    /// <inheritdoc cref="DynamicInputComponentBase.PropertyType"/>
    [Parameter]
    public Type? PropertyType { get; set; }

    /// <summary>
    /// The class for the current validation status
    /// </summary>
    [Parameter]
    public string ValidationStatusClass { get; set; } = string.Empty;

    /// <summary>
    /// A callback to notify the parent when the value has changed.
    /// </summary>
    [Parameter]
    public Action<Result<object?>>? ParentCallback { get; set; }

    /// <summary>
    /// Binding for the input element value.
    /// </summary>
    protected T Value
    {
        get => GetValue();
        set => SetValue(value);
    }

    /// <summary>
    /// Get the value as the appropriate type.
    /// </summary>
    protected virtual T GetValue()
    {
        if (PropertyValue is T tValue)
            return tValue;
        throw new InvalidOperationException($"The property type was not {typeof(T)}.");
    }

    /// <summary>
    /// Set the value via the parent callback.
    /// </summary>
    protected virtual void SetValue(T value)
    {
        Result<object?> result = value;
        ParentCallback?.Invoke(result);
    }
}
