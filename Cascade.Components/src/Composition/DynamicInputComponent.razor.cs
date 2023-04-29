using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Cascade.Components.Composition;

/// <summary>
/// A <see cref="IComponent"/> to dynamically select the appropriate input type for the given <see cref="DynamicInputComponentBase.Input"/>.
/// </summary>
public class DynamicInputComponentBase : ComponentBase
{
    [Inject]
    private ILogger<DynamicInputComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="InputProxy"/>
    [Parameter]
    public InputProxy Input { get; set; } = default!;

    /// <summary>
    /// The property value as a <see cref="string"/>.
    /// </summary>
    protected string ValueAsString
    {
        get => Input.GetValueAs<string>();
        set => Input.SetValue(value);
    }

    /// <summary>
    /// The property value as a <see cref="int"/>.
    /// </summary>
    protected int ValueAsInteger
    {
        get => Input.GetValueAs<int>();
        set => Input.SetValue(value);
    }

    /// <summary>
    /// The property value as a <see cref="double"/>.
    /// </summary>
    protected double ValueAsDouble
    {
        get => Input.GetValueAs<double>();
        set => Input.SetValue(value);
    }

    /// <summary>
    /// The property value as an <see cref="Enum"/>.
    /// </summary>
    protected Enum ValueAsEnum
    {
        get => Input.GetValueAs<Enum>();
        set => Input.SetValue(value);
    }

    /// <summary>
    /// The property value as a <see cref="bool"/>.
    /// </summary>
    protected bool ValueAsBoolean
    {
        get => Input.GetValueAs<bool>();
        set => Input.SetValue(value);
    }

    /// <summary>
    /// The property value as an <see cref="object"/>.
    /// </summary>
    protected object? ValueAsObject
    {
        get => Input.GetValue();
        set => Input.SetValue(value);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Input is null)
            throw new("State is not set;");
    }

    /// <summary>
    /// Validate the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>An array of errors.</returns>
    protected string[] ValidateAs<T>(T value)
    {
        Logger.LogDebug($"{nameof(ValidateAs)} called.");
        return Input.Validate().Errors;
    }
}
