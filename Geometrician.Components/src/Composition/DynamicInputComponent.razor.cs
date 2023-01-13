using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Composition;

public class DynamicInputComponentBase : ComponentBase
{
    [Inject]
    private ILogger<DynamicInputComponent> Logger { get; set; } = default!;

    [Parameter]
    public InputProxy Input { get; set; } = default!;

    protected string ValueAsString
    {
        get => Input.GetValueAs<string>();
        set => Input.SetValue(value);
    }

    protected int ValueAsInteger
    {
        get => Input.GetValueAs<int>();
        set => Input.SetValue(value);
    }

    protected double ValueAsDouble
    {
        get => Input.GetValueAs<double>();
        set => Input.SetValue(value);
    }

    protected Enum ValueAsEnum
    {
        get => Input.GetValueAs<Enum>();
        set => Input.SetValue(value);
    }

    protected bool ValueAsBoolean
    {
        get => Input.GetValueAs<bool>();
        set => Input.SetValue(value);
    }

    protected object? ValueAsObject
    {
        get => Input.GetValue();
        set => Input.SetValue(value);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Input is null)
            throw new("State is not set;");
    }

    protected IEnumerable<string> ValidateAs<T>(T value)
    {
        Logger.LogDebug($"{nameof(ValidateAs)} called.");
        return Input.Validate().Errors;
    }
}
