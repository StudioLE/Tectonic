using Cascade.Components.Composition;
using Cascade.Components.Shared;
using Cascade.Components.Visualization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Cascade.Components.Pages;

/// <summary>
/// Code-behind the <see cref="RunnerPage"/>.
/// </summary>
public class RunnerPageBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<RunnerPage> Logger { get; set; } = default!;

    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <inheritdoc cref="CompositionState"/>
    [Inject]
    protected CompositionState Resolver { get; set; } = null!;

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    protected VisualizationState Visualization { get; set; } = default!;

    /// <summary>
    /// The key of the currently selected assembly.
    /// This is the assembly name without a .dll extension.
    /// </summary>
    [Parameter]
    public string? AssemblyKey { get; set; }

    /// <summary>
    /// The key of the currently selected activity.
    /// This is the activity name without an assembly prefix.
    /// </summary>
    [Parameter]
    public string? ActivityKey { get; set; }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)}() called. Assembly: {AssemblyKey ?? "[null]"}; Activity: {ActivityKey ?? "[null]"};");
        Visualization.OutcomesChanged += NotifyStateHasChanged;
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        Resolver.SelectedAssemblyKey = AssemblyKey ?? string.Empty;
        Resolver.SelectedActivityKey = ActivityKey ?? string.Empty;
    }

    private async void NotifyStateHasChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Visualization.OutcomesChanged -= NotifyStateHasChanged;
    }
}
