﻿using System.Collections.Specialized;
using System.Reflection;
using Geometrician.Core.Samples;
using Lineweights.Drawings.Samples;
using Lineweights.Flex.Samples;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Execution;

public class RunnerComponentBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<RunnerComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="ExecutionState"/>
    [Inject]
    protected ExecutionState Execution { get; set; } = null!;

    /// <summary>
    /// The assemblies to populate the <see cref="AssemblySelectionComponent"/> with
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<Assembly> Assemblies { get; set; } = Array.Empty<Assembly>();

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
        Execution.Messages.CollectionChanged += OnMessagesChanged;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Execution.SelectedAssemblyKey = AssemblyKey ?? string.Empty;
        Execution.SelectedActivityKey = ActivityKey ?? string.Empty;
        if (!Assemblies.Any())
            Assemblies = new[]
            {
                typeof(SheetSample).Assembly,
                typeof(AssetTypes).Assembly,
                typeof(WallFlemishBond).Assembly
            };
        foreach (Assembly assembly in Assemblies)
            Execution.LoadedAssemblies.TryAdd(assembly.GetName().Name ?? "Unnamed", assembly);
    }

    private async void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Logger.LogDebug($"{nameof(OnMessagesChanged)}() called.");
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Execution.Messages.CollectionChanged -= OnMessagesChanged;
    }
}
