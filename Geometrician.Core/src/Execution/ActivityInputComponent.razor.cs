using System.Reflection;
using StudioLE.Core.Results;
using Geometrician.Core.Shared;
using Lineweights.Core.Documents;
using Lineweights.PDF;
using Lineweights.SVG;
using Lineweights.Workflows;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Execution;

public class ActivityInputComponentBase : ComponentBase, IDisposable
{
    private ActivityCommand? _activity;

    /// <inheritdoc cref="ILogger"/>
    [Inject]
    public ILogger<ActivitySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    public RunnerState State { get; set; } = null!;

    /// <inheritdoc cref="Geometrician.Core.Shared.AssetState"/>
    [Inject]
    protected AssetState AssetState { get; set; } = default!;

    /// <inheritdoc cref="IStorageStrategy"/>
    [Inject]
    protected IStorageStrategy StorageStrategy { get; set; } = default!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    protected IActivityFactory Factory { get; set; } = default!;

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

    protected async void BuildAndExecute()
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

        DocumentInformation doc = new()
        {
            Name = _activity.Name,
            Description = $"Executed {State.SelectedActivityKey} from {State.SelectedAssemblyKey}."
        };
        // TODO: If this is created via DI then the Storage Strategy will be injected automatically.
        AssetBuilder builder = new AssetBuilder()
            .SetStorageStrategy(StorageStrategy)
            .SetDocumentInformation(doc)
            .ConvertModelToGlb()
            .ExtractViewsAndConvertToSvg()
            .ExtractSheetsAndConvertToPdf();
        IResult<IReadOnlyCollection<Asset>> assetsResult = outputs.TryGetPropertyValue<IReadOnlyCollection<Asset>>("Assets");
        if (assetsResult is Success<IReadOnlyCollection<Asset>> assetsSuccess)
        {
            foreach (Asset child in assetsSuccess.Value)
                await StorageStrategy.RecursiveWriteLocalFilesToStorage(child);
            builder.AddAssets(assetsSuccess.Value.ToArray());
        }

        Asset asset = await builder.Build(successModel);
        AssetState.Assets.Add(asset);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _activity?.Dispose();
    }
}
