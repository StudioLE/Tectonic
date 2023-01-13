using System.Reflection;
using Geometrician.Components.Composition;
using Geometrician.Components.Shared;
using StudioLE.Core.Results;
using Geometrician.Components.Visualization;
using Geometrician.Core;
using Geometrician.Core.Execution;
using Lineweights.Core.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Geometrician.Components.Execution;

public class ActivityInputComponentBase : ComponentBase, IDisposable
{
    private ActivityCommand? _activity;

    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ActivitySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="ExecutionState"/>
    [Inject]
    private ExecutionState Execution { get; set; } = null!;

    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    private VisualizationState Visualization { get; set; } = default!;

    /// <inheritdoc cref="IStorageStrategy"/>
    [Inject]
    private IStorageStrategy StorageStrategy { get; set; } = default!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    private IActivityFactory Factory { get; set; } = default!;

    protected IReadOnlyCollection<InputPackProxy> InputPacks { get; private set; } = Array.Empty<InputPackProxy>();

    protected MudForm Form { get; set; } = default!;

    protected string[] Errors { get; set; } = Array.Empty<string>();

    protected bool IsValid { get; set; }

    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)} called. Activity: {Execution.SelectedActivityKey} Assembly: {Execution.SelectedActivityKey}");
        if (!Execution.TryGetAssemblyByKey(Execution.SelectedAssemblyKey, out Assembly? assembly))
        {
            Execution.SelectedAssemblyKey = string.Empty;
            return;
        }

        IResult<ActivityCommand> result = Factory.TryCreateByKey(assembly!, Execution.SelectedActivityKey);
        if (result is not Success<ActivityCommand> success)
        {
            Execution.ShowError(Logger, "Failed to load activity. Method does not exist.");
            Execution.SelectedActivityKey = string.Empty;
            return;
        }
        _activity = success;
        InputPacks = _activity
            .Inputs
            .Select(x => new InputPackProxy(x))
            .ToArray();
    }

    protected async Task SubmitAsync()
    {
        Logger.LogDebug($"{nameof(SubmitAsync)} called");

        await Form.Validate();

        if (IsValid)
        {
            Logger.LogDebug("Validation passed.");
            BuildAndExecute();
        }
        else
            Logger.LogDebug("Validation failed.");

        // Process the valid form
    }

    protected void BuildAndExecute()
    {
        Execution.Messages.Clear();
        Logger.LogDebug($"{nameof(BuildAndExecute)} called.");

        if (_activity is null)
        {
            Execution.ShowError(Logger, "Failed to load activity. Method does not exist.");
            return;
        }

        // TODO: Move this logic to workflow execution

        object outputs;
        Logger.LogDebug("Execution started.");
        // TODO: Add a timer
        // TODO: Add task cancellation
        // TODO: Add spinner during execution.
        try
        {
            outputs = _activity.Execute();
        }
        catch (TargetInvocationException e)
        {
            Execution.ShowError(Logger, e.InnerException ?? e, "Execution failed");
            return;
        }
        catch (Exception e)
        {
            Execution.ShowError(Logger, e, "Execution failed");
            return;
        }
        Logger.LogDebug("Execution completed.");

        IResult<Model> model = outputs.TryGetPropertyValue<Model>("Model");
        if (model is not Success<Model> successModel)
        {
            Execution.ShowWarning(Logger, "Activity output was not a model.");
            return;
        }

        Outcome outcome = new()
        {
            Name = _activity.Name ?? string.Empty,
            Description = $"Executed {Execution.SelectedActivityKey} from {Execution.SelectedAssemblyKey}."
        };

        Visualization.AddOutcome(outcome, outputs);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _activity?.Dispose();
    }
}
