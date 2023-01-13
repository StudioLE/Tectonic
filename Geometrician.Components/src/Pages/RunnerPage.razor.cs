using System.Collections.Specialized;
using System.Reflection;
using Geometrician.Components.Composition;
using Geometrician.Components.Shared;
using Geometrician.Components.Visualization;
using Geometrician.Core.Samples;
using Lineweights.Drawings.Samples;
using Lineweights.Flex.Samples;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Pages;

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

    /// <summary>
    /// The assemblies to populate the <see cref="AssemblyResolverComponent"/> with
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<Assembly> Assemblies { get; set; } = Array.Empty<Assembly>();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)}() called. Assembly: {AssemblyKey ?? "[null]"}; Activity: {ActivityKey ?? "[null]"};");
        Visualization.OutcomesChanged += NotifyStateHasChanged;
        Resolver.Messages.CollectionChanged += OnMessagesChanged;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Resolver.SelectedAssemblyKey = AssemblyKey ?? string.Empty;
        Resolver.SelectedActivityKey = ActivityKey ?? string.Empty;
        if (!Assemblies.Any())
            Assemblies = new[]
            {
                typeof(SheetSample).Assembly,
                typeof(AssetTypes).Assembly,
                typeof(WallFlemishBond).Assembly
            };
        foreach (Assembly assembly in Assemblies)
            Resolver.LoadedAssemblies.TryAdd(assembly.GetName().Name ?? "Unnamed", assembly);
    }

    private async void NotifyStateHasChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Logger.LogDebug($"{nameof(OnMessagesChanged)}() called.");
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Visualization.OutcomesChanged -= NotifyStateHasChanged;
        Resolver.Messages.CollectionChanged -= OnMessagesChanged;
    }
}
