using Geometrician.Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Geometrician.Components.Composition;

public class DynamicFormComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<DynamicFormComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

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

    protected IReadOnlyCollection<ObjectState> States { get; private set; } = Array.Empty<ObjectState>();

    protected MudForm Form { get; set; } = default!;

    protected string[] Errors { get; set; } = Array.Empty<string>();

    protected bool IsValid { get; set; }

    protected override void OnInitialized()
    {
        States = Inputs
            .Select(x => new ObjectState(x))
            .ToArray();
    }

    protected async Task SubmitAsync()
    {
        Logger.LogDebug($"{nameof(SubmitAsync)} called");

        await Form.Validate();

        if (IsValid)
        {
            Logger.LogDebug("Validation passed.");
            OnValidSubmit.Invoke();
        }
        else
            Logger.LogDebug("Validation failed.");

        // Process the valid form
    }
}
