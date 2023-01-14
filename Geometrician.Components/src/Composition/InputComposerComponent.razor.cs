using System.Reflection;
using Geometrician.Components.Shared;
using Geometrician.Components.Visualization;
using Geometrician.Core;
using Geometrician.Core.Configuration;
using Geometrician.Core.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using StudioLE.Core.Results;

namespace Geometrician.Components.Composition;

/// <summary>
/// A <see cref="IComponent"/> to set inputs for an activity.
/// </summary>
public class InputComposerComponentBase : ComponentBase, IDisposable
{
    private ActivityCommand? _activity;

    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ActivityResolverComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="Geometrician.Core.Configuration.AssemblyResolver"/>
    [Inject]
    private AssemblyResolver AssemblyResolver { get; set; } = null!;

    /// <inheritdoc cref="CompositionState"/>
    [Inject]
    private CompositionState Composition { get; set; } = null!;

    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    private VisualizationState Visualization { get; set; } = default!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    private IActivityFactory Factory { get; set; } = default!;

    /// <summary>
    /// Proxies for each input pack for the activity.
    /// </summary>
    protected IReadOnlyCollection<InputPackProxy> InputPacks { get; private set; } = Array.Empty<InputPackProxy>();

    /// <summary>
    /// The form.
    /// </summary>
    protected MudForm Form { get; set; } = default!;

    /// <summary>
    /// The error messages.
    /// </summary>
    protected string[] Errors { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Are the form inputs valid?
    /// </summary>
    protected bool IsValid { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)} called. Activity: {Composition.SelectedActivityKey} Assembly: {Composition.SelectedAssemblyKey}");
        Assembly? assembly = AssemblyResolver.ResolveByName(Composition.SelectedAssemblyKey);
        if (assembly is null)
        {
            Composition.ShowError(Logger, "Failed to load assembly. Key not found: " + Composition.SelectedActivityKey);
            return;
        }

        IResult<ActivityCommand> result = Factory.TryCreateByKey(assembly, Composition.SelectedActivityKey);
        if (result is not Success<ActivityCommand> success)
        {
            Composition.ShowError(Logger, "Failed to load activity. Method does not exist.");
            Composition.SelectedActivityKey = string.Empty;
            return;
        }
        _activity = success;
        InputPacks = _activity
            .Inputs
            .Select(x => new InputPackProxy(x))
            .ToArray();
    }

    /// <summary>
    /// The form submit action.
    /// </summary>
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

    /// <summary>
    /// Build the <see cref="ActivityCommand"/>, execute it, and process the results.
    /// </summary>
    private void BuildAndExecute()
    {
        Composition.Messages.Clear();
        Logger.LogDebug($"{nameof(BuildAndExecute)} called.");

        if (_activity is null)
        {
            Composition.ShowError(Logger, "Failed to load activity. Method does not exist.");
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
            Composition.ShowError(Logger, e.InnerException ?? e, "Execution failed");
            return;
        }
        catch (Exception e)
        {
            Composition.ShowError(Logger, e, "Execution failed");
            return;
        }
        Logger.LogDebug("Execution completed.");

        IResult<Model> model = outputs.TryGetPropertyValue<Model>("Model");
        if (model is not Success<Model> successModel)
        {
            Composition.ShowWarning(Logger, "Activity output was not a model.");
            return;
        }

        Outcome outcome = new()
        {
            Name = _activity.Name ?? string.Empty,
            Description = $"Executed {Composition.SelectedActivityKey} from {Composition.SelectedAssemblyKey}."
        };

        Visualization.AddOutcome(outcome, outputs);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _activity?.Dispose();
    }
}
