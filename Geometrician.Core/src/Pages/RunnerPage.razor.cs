﻿using Geometrician.Core.Shared;
using Geometrician.Core.Visualization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Pages;

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

    /// <inheritdoc cref="VisualizationState"/>
    [Inject]
    protected VisualizationState State { get; set; } = default!;

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

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)}() called. Assembly: {AssemblyKey ?? "[null]"}; Activity: {ActivityKey ?? "[null]"};");
        State.OutcomesChanged += NotifyStateHasChanged;
    }

    /// <summary>
    /// Notify that the state has changed.
    /// </summary>
    private async void NotifyStateHasChanged(object? sender, EventArgs e)
    {
        Logger.LogDebug($"{nameof(NotifyStateHasChanged)}() called.");
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        State.OutcomesChanged -= NotifyStateHasChanged;
    }
}
