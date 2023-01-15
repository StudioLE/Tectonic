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
    private IActivity? _activity;

    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ActivityResolverComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="Geometrician.Core.Configuration.AssemblyResolver"/>
    [Inject]
    private AssemblyResolver AssemblyResolver { get; set; } = null!;

    /// <inheritdoc cref="CompositionState"/>
    [Inject]
    private CompositionState Composition { get; set; } = null!;

    /// <inheritdoc cref="CommunicationState"/>
    [Inject]
    private CommunicationState Communication { get; set; } = null!;

    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    private VisualizationState Visualization { get; set; } = default!;

    /// <inheritdoc cref="IActivityResolver"/>
    [Inject]
    private IActivityResolver Resolver { get; set; } = default!;

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

            string message = "Failed to load assembly. Key not found: " + Composition.SelectedAssemblyKey;
            Logger.LogError(message);
            Communication.ShowError(message);
            return;
        }

        IResult<IActivity> result = Resolver.Resolve(assembly, Composition.SelectedActivityKey);
        if (result is not Success<IActivity> success)
        {
            string message = "Failed to load activity. Key not found: " + Composition.SelectedActivityKey;
            Logger.LogError(message);
            Communication.ShowError(message);
            Composition.SelectedActivityKey = string.Empty;
            return;
        }
        _activity = success.Value;
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
    /// Build the <see cref="IActivity"/>, execute it, and process the results.
    /// </summary>
    private void BuildAndExecute()
    {
        Logger.LogDebug($"{nameof(BuildAndExecute)} called.");

        if (_activity is null)
        {
            string message = "Failed to load activity. Method does not exist.";
            Logger.LogError(message);
            Communication.ShowError(message);
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
            string message = "Execution failed.";
            Logger.LogError(e.InnerException ?? e, message);
            Communication.ShowError(message);
            return;
        }
        catch (Exception e)
        {
            string message = "Execution failed.";
            Logger.LogError(e, message);
            Communication.ShowError(message);
            return;
        }
        Logger.LogDebug("Execution completed.");

        IResult<Model> model = outputs.TryGetPropertyValue<Model>("Model");
        if (model is not Success<Model> successModel)
        {
            string message = "Activity output was not a model.";
            Logger.LogWarning(message);
            Communication.ShowWarning(message);
            return;
        }

        Outcome outcome = new()
        {
            Name = _activity.Name,
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
