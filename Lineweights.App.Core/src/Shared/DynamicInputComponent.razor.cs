using System.Reflection;
using Ardalis.Result;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Lineweights.App.Core.Shared;

public class DynamicInputComponentBase : ComponentBase, IDisposable
{
    #region Fields

    private EditContext _editContext = default!;
    private FieldIdentifier _fieldIdentifier;
    private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
    private bool _previousParsingAttemptFailed;
    private ValidationMessageStore? _parsingValidationMessages;
    private Type? _type;

    #endregion

    #region Parameters

    /// <summary>
    /// The instance of the inputs object.
    /// </summary>
    [Parameter]
    public object Instance { get; set; } = default!;

    /// <summary>
    /// The property of the inputs object.
    /// </summary>
    [Parameter]
    public PropertyInfo Property { get; set; } = default!;

    [CascadingParameter]
    private EditContext CascadedEditContext { get; set; } = default!;

    /// <summary>
    /// A collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The display name for this field.
    /// <para>This value is used when generating error messages when the input value fails to parse correctly.</para>
    /// </summary>
    [Parameter]
    public string? DisplayName { get; set; }

    #endregion

    #region Init

    public DynamicInputComponentBase()
    {
        _validationStateChangedHandler = OnValidateStateChanged;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        _editContext = CascadedEditContext ?? throw new InvalidOperationException($"{GetType()} requires a cascading parameter " + $"of type {nameof(_editContext)}. For example, you can use {GetType().FullName} inside " + $"an {nameof(EditForm)}.");
        _fieldIdentifier = new(Instance, Property.Name);
        _editContext.OnValidationStateChanged += _validationStateChangedHandler;

        _type = Property.PropertyType;
        Type? underlyingType = Nullable.GetUnderlyingType(_type);
        if (underlyingType is not null)
            _type = underlyingType;

        if (_type == typeof(double) || _type == typeof(int))
            Type = InputType.Number;
        else if (_type == typeof(string))
            Type = InputType.Text;
        else if (typeof(Enum).IsAssignableFrom(_type))
        {
            Type = InputType.Select;
            Options = Enum.GetNames(_type);
        }
        else if (_type == typeof(bool))
            Type = InputType.Checkbox;
        else
            Type = InputType.Unknown;


        UpdateAdditionalValidationAttributes();
    }

    #endregion

    #region Value Properties

    private object? Value {
        get => Property.GetValue(Instance);
        set  {
            bool hasChanged = !EqualityComparer<object>.Default.Equals(value, Value);
            if (hasChanged)
            {
                if (Property is null)
                    throw new("Property was null..");
                Property.SetValue(Instance, value);
                _editContext.NotifyFieldChanged(_fieldIdentifier);
            }
        }
    }

    protected string? ValueAsString {
        get => GetCurrentValueAsString();
        set => SetCurrentValueAsString(value);
    }

    /// <summary>
    /// Gets or sets the current value of the input, represented as a string.
    /// </summary>
    private string? GetCurrentValueAsString()
    {
        return Value?.ToString();
    }

    /// <summary>
    /// Gets or sets the current value of the input, represented as a string.
    /// </summary>
    private void SetCurrentValueAsString(string? value)
    {
        _parsingValidationMessages?.Clear();

        Result<object?> parsed = TryParseValueFromString(value);
        if (parsed.IsSuccess)
            Value = parsed.Value;
        else
        {
            _parsingValidationMessages ??= new(_editContext);
            _parsingValidationMessages.Add(_fieldIdentifier, string.Join(' ', parsed.Errors));

            // Since we're not writing to CurrentValue, we'll need to notify about modification from here
            _editContext.NotifyFieldChanged(_fieldIdentifier);
        }

        // We can skip the validation notification if we were previously valid and still are
        if (parsed.IsSuccess && !_previousParsingAttemptFailed)
            return;

        _editContext.NotifyValidationStateChanged();
        _previousParsingAttemptFailed = !parsed.IsSuccess;
    }

    private Result<object?> TryParseValueFromString(string? value)
    {
        Func<object?, Result<object?>> success = Result<object?>.Success;
        Func<string, Result<object?>> failure = str => Result<object?>.Error($"The {DisplayName ?? _fieldIdentifier.FieldName} field must be {str}.");

        if (_type == typeof(string))
            return success.Invoke(value);
        if (_type == typeof(int))
            return int.TryParse(value, out int parsed)
                ? success.Invoke(parsed)
                : failure.Invoke("an integer");
        if (_type == typeof(double))
            return double.TryParse(value, out double parsed)
                ? success.Invoke(parsed)
                : failure.Invoke("a number");
        if (typeof(Enum).IsAssignableFrom(_type))
            return Enum.TryParse(_type, value, out object? parsed)
                ? success.Invoke(parsed)
                : failure.Invoke("a boolean");

        // TODO: Replace with Logger
        throw new("Unsupported type.");
    }

    protected bool ValueAsBoolean {
        get => GetCurrentValueAsBoolean();
        set => SetCurrentValueAsBoolean(value);
    }

    /// <summary>
    /// Gets or sets the current value of the input, represented as a string.
    /// </summary>
    private bool GetCurrentValueAsBoolean()
    {
        return Value as bool? ?? throw new("Value was not a boolean.");
    }

    /// <summary>
    /// Gets or sets the current value of the input, represented as a string.
    /// </summary>
    private void SetCurrentValueAsBoolean(bool value)
    {
        _parsingValidationMessages?.Clear();
        Value = value;
        // We can skip the validation notification if we were previously valid and still are
        if (!_previousParsingAttemptFailed)
            return;
        _editContext.NotifyValidationStateChanged();
        _previousParsingAttemptFailed = false;
    }

    #endregion

    #region Properties

    protected enum InputType
    {
        Unknown,
        Number,
        Text,
        Select,
        Checkbox
    }

    protected InputType Type { get; private set; }= InputType.Unknown;

    protected IReadOnlyCollection<string> Options { get; set; } = Array.Empty<string>();

    #endregion

    #region Attributes

    protected string ValidationStatusClass => _editContext.FieldCssClass(_fieldIdentifier);

    private void UpdateAdditionalValidationAttributes()
    {
        bool hasAriaInvalidAttribute = AdditionalAttributes?.ContainsKey("aria-invalid") ?? false;
        if (_editContext.GetValidationMessages(_fieldIdentifier).Any())
        {
            if (hasAriaInvalidAttribute)

                // Do not overwrite the attribute value
                return;

            if (ConvertToDictionary(AdditionalAttributes, out var additionalAttributes))
                AdditionalAttributes = additionalAttributes;

            // To make the `Input` components accessible by default
            // we will automatically render the `aria-invalid` attribute when the validation fails
            // value must be "true" see https://www.w3.org/TR/wai-aria-1.1/#aria-invalid
            additionalAttributes["aria-invalid"] = "true";
        }
        else if (hasAriaInvalidAttribute)
        {
            // No validation errors. Need to remove `aria-invalid` if it was rendered already

            if (AdditionalAttributes!.Count == 1)

                // Only aria-invalid argument is present which we don't need any more
                AdditionalAttributes = null;
            else
            {
                if (ConvertToDictionary(AdditionalAttributes, out var additionalAttributes))
                    AdditionalAttributes = additionalAttributes;

                additionalAttributes.Remove("aria-invalid");
            }
        }
    }

    private static bool ConvertToDictionary(IReadOnlyDictionary<string, object>? source, out Dictionary<string, object> result)
    {
        result = source switch
        {
            null => new(),
            Dictionary<string, object> currentDictionary => currentDictionary,
            _ => source.ToDictionary(item => item.Key, item => item.Value)
        };
        return source is not Dictionary<string, object>;
    }

    #endregion

    #region Misc

    private void OnValidateStateChanged(object? sender, ValidationStateChangedEventArgs eventArgs)
    {
        UpdateAdditionalValidationAttributes();
        StateHasChanged();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _editContext.OnValidationStateChanged -= _validationStateChangedHandler;
    }

    #endregion
}
