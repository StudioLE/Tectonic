using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows;
using Lineweights.Workflows.Assets;
using Lineweights.Workflows.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StudioLE.Core.System;

namespace Lineweights.App.Core.Shared;

public class ActivityInputComponentBase : ComponentBase
{
    private MethodInfo? _method;

    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    public ILogger<ActivitySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    public RunnerState State { get; set; } = null!;

    /// <inheritdoc cref="AssetState"/>
    [Inject]
    protected AssetState AssetState { get; set; } = default!;

    /// <inheritdoc cref="IStorageStrategy"/>
    [Inject]
    protected IStorageStrategy StorageStrategy { get; set; } = default!;

    /// <summary>
    /// The inputs for the activity.
    /// </summary>
    public IReadOnlyCollection<object> Inputs { get; private set; } = Array.Empty<object>();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)} called. Activity: {State.SelectedActivityKey} Assembly: {State.SelectedActivityKey}");
        if(!State.TryGetAssemblyByKey(State.SelectedAssemblyKey, out Assembly? assembly))
        {
            State.SelectedAssemblyKey = string.Empty;
            return;
        }

        IEnumerable<MethodInfo> methods = ActivityHelpers.GetActivityMethods(assembly!);

        _method = methods
            .FirstOrDefault(x => ActivityHelpers.GetActivityKey(x) == State.SelectedActivityKey);

        if (_method is null)
        {
            State.ShowError(Logger, "Failed to load activity. Method does not exist.");
            State.SelectedActivityKey = string.Empty;
            return;
        }

        Inputs = ActivityHelpers.CreateParameterInstances(_method);

    }

    protected async void BuildAndExecute()
    {
        State.Messages.Clear();
        Logger.LogDebug($"{nameof(BuildAndExecute)} called.");

        if (_method is null)
        {
            State.ShowError(Logger, "Failed to load activity. Method does not exist.");
            return;
        }

        ActivityCommand command = ActivityCommand.CreateByMethod(_method!, Inputs.ToArray());


        // TODO: Move this logic to workflow execution

        Result<object> executionResult;
        Logger.LogDebug("Execution started.");
        // TODO: Add a timer
        // TODO: Add task cancellation
        // TODO: Add spinner during execution.
        try
        {
            executionResult = command.Execute();
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

        if (!executionResult.IsSuccess)
        {
            State.ShowWarning(Logger, executionResult.Errors.Join());
            return;
        }

        object outputs = executionResult.Value;

        Result<Model> model = TryGetPropertyValue<Model>(outputs, "Model");
        if (!model.IsSuccess)
        {
            State.ShowWarning(Logger, "Activity output was not a model.");
            return;
        }

        DocumentInformation doc = new()
        {
            Name = command.Name,
            Description = $"Executed {State.SelectedActivityKey} from {State.SelectedAssemblyKey}."
        };
        AssetBuilder builder = AssetBuilder.Default(StorageStrategy, model.Value, doc);

        Result<IReadOnlyCollection<Asset>> assets = TryGetPropertyValue<IReadOnlyCollection<Asset>>(outputs, "Assets");
        if (assets.IsSuccess)
        {
            foreach (Asset child in assets.Value)
                await StorageStrategy.RecursiveWriteLocalFilesToStorage(child);
            builder.AddAssets(assets.Value.ToArray());
        }

        Asset asset = await builder.Build();
        AssetState.Assets.Add(asset);
    }

    private static Result<T> TryGetPropertyValue<T>(object @this, string propertyName, BindingFlags? flags = null)
    {
        Type type = @this.GetType();
        PropertyInfo? property = flags is null
            ? type.GetProperty(propertyName)
            : type.GetProperty(propertyName, (BindingFlags)flags);
        if(property is null)
            return Result<T>.Error("Property does not exist");
        object? value = property.GetValue(@this);
        if (value is T tValue)
            return tValue;
        return Result<T>.Error($"Property type was {value?.GetType()}.");
    }
}
