using System.Reflection;
using Geometrician.Components.Shared;
using Geometrician.Core.Configuration;
using Geometrician.Core.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Composition;

/// <summary>
/// A <see cref="IComponent"/> to select an activity from an assembly.
/// </summary>
public class ActivityResolverComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ActivityResolverComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="Geometrician.Core.Configuration.AssemblyResolver"/>
    [Inject]
    private AssemblyResolver AssemblyResolver { get; set; } = null!;

    /// <inheritdoc cref="CompositionState"/>
    [Inject]
    private CompositionState Composition { get; set; } = null!;

    /// <inheritdoc cref="CommunicationState"/>
    [Inject]
    private CommunicationState Communication { get; set; } = null!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    private IActivityFactory Factory { get; set; } = default!;

    /// <summary>
    /// Binding for the options of the assembly select element.
    /// </summary>
    protected IReadOnlyCollection<string> ActivitySelectOptions { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Binding for the activity select element value.
    /// </summary>
    protected string ActivitySelectValue { get; set; } = string.Empty;

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
        ActivitySelectOptions = Factory.AllActivityKeysInAssembly(assembly).ToArray();
        if (ActivitySelectOptions.Any())
            ActivitySelectValue = ActivitySelectOptions.First();
    }

    /// <summary>
    /// Set the activity according to <see cref="ActivitySelectValue"/> by changing page.
    /// </summary>
    protected void SetActivity()
    {
        Logger.LogDebug($"{nameof(SetActivity)} called with {ActivitySelectValue}.");
        Navigation.NavigateTo($"/run/{Composition.SelectedAssemblyKey}/{ActivitySelectValue}");
    }
}
