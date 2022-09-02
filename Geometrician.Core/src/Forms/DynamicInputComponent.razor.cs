using System.Reflection;
using Ardalis.Result;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Geometrician.Core.Forms;

public class DynamicInputComponentBase : ComponentBase, IDisposable
{
    #region Fields

    private EditContext _editContext = default!;
    private FieldIdentifier _fieldIdentifier;
    private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
    private bool _previousParsingAttemptFailed;
    private ValidationMessageStore? _parsingValidationMessages;

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
        PropertyType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
        UpdateAdditionalValidationAttributes();
    }

    #endregion

    #region Property Value

    /// <summary>
    /// The type (or underlying type for nullable elements) of the property value.
    /// </summary>
    protected Type? PropertyType { get; private set; }

    protected object? PropertyValue => Property.GetValue(Instance);

    private void SetValue(object? value)
    {
        bool hasChanged = !EqualityComparer<object>.Default.Equals(value, PropertyValue);
        if (hasChanged)
        {
            if (Property is null)
                throw new("Property was null..");
            Property.SetValue(Instance, value);
            _editContext.NotifyFieldChanged(_fieldIdentifier);
        }
    }

    protected void ValidateAndSetValue(Result<object?> result)
    {
        _parsingValidationMessages?.Clear();

        if (result.IsSuccess)
            SetValue(result.Value);
        else
        {
            _parsingValidationMessages ??= new(_editContext);
            _parsingValidationMessages.Add(_fieldIdentifier, string.Join(' ', result.Errors));

            // Since we're not writing to CurrentValue, we'll need to notify about modification from here
            _editContext.NotifyFieldChanged(_fieldIdentifier);
        }

        // We can skip the validation notification if we were previously valid and still are
        if (result.IsSuccess && !_previousParsingAttemptFailed)
            return;

        _editContext.NotifyValidationStateChanged();
        _previousParsingAttemptFailed = !result.IsSuccess;
    }

    #endregion

    #region Validation Attributes

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
