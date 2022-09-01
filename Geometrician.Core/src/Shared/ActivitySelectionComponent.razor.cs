using System.Reflection;
using Lineweights.Workflows.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Shared;

public class ActivitySelectionComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    public ILogger<ActivitySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    public RunnerState State { get; set; } = null!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    protected IActivityFactory Factory { get; set; } = default!;

    /// <summary>
    /// Binding for the options of the assembly select element.
    /// </summary>
    public IReadOnlyCollection<string> ActivitySelectOptions { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Binding for the activity select element value.
    /// </summary>
    public string ActivitySelectValue { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)} called. Activity: {State.SelectedActivityKey} Assembly: {State.SelectedActivityKey}");
        if(!State.TryGetAssemblyByKey(State.SelectedAssemblyKey, out Assembly? assembly))
            return;
        ActivitySelectOptions = Factory.AllActivityKeysInAssembly(assembly!).ToArray();
        if (ActivitySelectOptions.Any())
            ActivitySelectValue = ActivitySelectOptions.First();
    }

    protected void SetActivity()
    {
        Logger.LogDebug($"{nameof(SetActivity)} called with {ActivitySelectValue}.");
        Navigation.NavigateTo($"/run/{State.SelectedAssemblyKey}/{ActivitySelectValue}");
    }
}
