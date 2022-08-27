using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Shared;

public class DynamicFormComponentBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<DynamicFormComponent> Logger { get; set; } = default!;

    /// <summary>
    /// The submit action.
    /// </summary>
    [Parameter]
    public Action OnValidSubmit { get; set; } = default!;


    /// <summary>
    /// The inputs object.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<object> Inputs { get; set; } = default!;

    protected IReadOnlyCollection<EditContext> EditContexts { get; private set; }= Array.Empty<EditContext>();

    protected bool IsValid { get; private set; }

    protected override void OnInitialized()
    {
        EditContexts = Inputs
            .Select(x => new EditContext(x))
            .ToArray();

        foreach (EditContext context in EditContexts)
        {
            context.OnFieldChanged += OnFieldChanged;
            context.OnValidationStateChanged += OnValidationStateChanged;
        }

        UpdateIsValid();
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (!EditContexts.Any())
            return;
        UpdateIsValid();
    }

    private void OnValidationStateChanged(object? sender, ValidationStateChangedEventArgs e)
    {
        StateHasChanged();
    }

    private void UpdateIsValid()
    {
        bool isValid = EditContexts.All(x => x.Validate());
        if (isValid == IsValid)
            return;
        IsValid = isValid;
        StateHasChanged();
    }

    protected void Submit()
    {
        Logger.LogDebug($"{nameof(Submit)} called");

        UpdateIsValid();

        if (IsValid)
        {
            Logger.LogDebug("Validation passed.");
            OnValidSubmit.Invoke();
        }
        else
            Logger.LogDebug("Validation failed.");

        // Process the valid form
    }

    protected static IEnumerable<string> GetValidationMessages(EditContext editContext, object inputs, string propertyName)
    {
        IEnumerable<string> messages = editContext.GetValidationMessages(new FieldIdentifier(inputs, propertyName));
        return messages;
    }

    public void Dispose()
    {
        if (!EditContexts.Any())
            return;
        foreach(EditContext context in EditContexts)
        {
            context.OnFieldChanged -= OnFieldChanged;
            context.OnValidationStateChanged -= OnValidationStateChanged;
        }
    }
}
