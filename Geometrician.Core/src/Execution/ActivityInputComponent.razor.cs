using System.Reflection;
using StudioLE.Core.Results;
using Geometrician.Core.Visualization;
using Lineweights.Core.Documents;
using Lineweights.Workflows;
using Lineweights.Workflows.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Execution;

public class ActivityInputComponentBase : ComponentBase, IDisposable
{
    private ActivityCommand? _activity;

    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ActivitySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    private RunnerState State { get; set; } = null!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    private VisualizationState Visualization { get; set; } = default!;

    /// <inheritdoc cref="IStorageStrategy"/>
    [Inject]
    private IStorageStrategy StorageStrategy { get; set; } = default!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    private IActivityFactory Factory { get; set; } = default!;

    /// <summary>
    /// The inputs for the activity.
    /// </summary>
    public IReadOnlyCollection<object> Inputs { get; private set; } = Array.Empty<object>();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)} called. Activity: {State.SelectedActivityKey} Assembly: {State.SelectedActivityKey}");
        if (!State.TryGetAssemblyByKey(State.SelectedAssemblyKey, out Assembly? assembly))
        {
            State.SelectedAssemblyKey = string.Empty;
            return;
        }

        IResult<ActivityCommand> result = Factory.TryCreateByKey(assembly!, State.SelectedActivityKey);
        if (result is not Success<ActivityCommand> success)
        {
            State.ShowError(Logger, "Failed to load activity. Method does not exist.");
            State.SelectedActivityKey = string.Empty;
            return;
        }
        _activity = success;
        Inputs = _activity.Inputs;

    }

    protected void BuildAndExecute()
    {
        State.Messages.Clear();
        Logger.LogDebug($"{nameof(BuildAndExecute)} called.");

        if (_activity is null)
        {
            State.ShowError(Logger, "Failed to load activity. Method does not exist.");
            return;
        }
        _activity.Inputs = Inputs.ToArray();

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
            State.ShowError(Logger, e.InnerException ?? e, "Execution failed");
            return;
        }
        catch (Exception e)
        {
            State.ShowError(Logger, e, "Execution failed");
            return;
        }
        Logger.LogDebug("Execution completed.");

        IResult<Model> model = outputs.TryGetPropertyValue<Model>("Model");
        if (model is not Success<Model> successModel)
        {
            State.ShowWarning(Logger, "Activity output was not a model.");
            return;
        }

        Outcome outcome = new()
        {
            Name = _activity.Name ?? string.Empty,
            Description = $"Executed {State.SelectedActivityKey} from {State.SelectedAssemblyKey}."
        };

        Visualization.AddOutcome(outcome, outputs);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _activity?.Dispose();
    }
}
