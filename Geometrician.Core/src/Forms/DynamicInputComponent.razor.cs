using Ardalis.Result;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Forms;

public class DynamicInputComponentBase : ComponentBase
{
    [Inject]
    public ILogger<DynamicInputComponent> Logger { get; set; } = default!;

    [Parameter]
    public PropertyState State { get; set; } = default!;

    protected string Label { get; set; } = string.Empty;

    protected string HelperText { get; set; } = string.Empty;

    protected Type PropertyType { get; set; } = default!;

    protected string ValueAsString
    {
        get => State.GetValueAs<string>();
        set => State.SetValue(value);
    }

    protected int ValueAsInteger
    {
        get => State.GetValueAs<int>();
        set => State.SetValue(value);
    }

    protected double ValueAsDouble
    {
        get => State.GetValueAs<double>();
        set => State.SetValue(value);
    }

    protected Enum ValueAsEnum
    {
        get => State.GetValueAs<Enum>();
        set => State.SetValue(value);
    }

    protected bool ValueAsBoolean
    {
        get => State.GetValueAs<bool>();
        set => State.SetValue(value);
    }

    protected object? ValueAsObject
    {
        get => State.GetValue();
        set => State.SetValue(value);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (State is null)
            throw new("State is not set;");
        Label = State.Property.Name;
        HelperText = State.Property.Name;
        PropertyType = Nullable.GetUnderlyingType(State.Property.PropertyType) ?? State.Property.PropertyType;
    }

    protected IEnumerable<string> ValidateAs<T>(T value)
    {
        Logger.LogDebug($"{nameof(ValidateAs)} called.");
        Result<bool> result = State.Validate();
        return result.IsSuccess
            ? Array.Empty<string>()
            : result.Errors;
    }
}
