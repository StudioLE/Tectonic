using Ardalis.Result;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Forms;

public class EnumSelectComponentBase : InputComponentBase<string>
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    public ILogger<EnumSelectComponent> Logger { get; set; } = null!;

    protected IReadOnlyCollection<string> Options { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Get the value as the appropriate type.
    /// </summary>
    protected override string GetValue()
    {
        return PropertyValue?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Set the value via the parent callback.
    /// </summary>
    protected override void SetValue(string valueAsString)
    {
        Result<object?> result = Enum.TryParse(PropertyType!, valueAsString, out object? value)
            ? value
            : Result<object?>.Error("was not a valid value.");
        ParentCallback?.Invoke(result);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (PropertyType is null)
            throw new InvalidOperationException("PropertyType of enum was null.");
        if (!typeof(Enum).IsAssignableFrom(PropertyType))
            throw new InvalidOperationException("PropertyType was not assignable from enum.");
        Options = Enum.GetNames(PropertyType);
    }
}
