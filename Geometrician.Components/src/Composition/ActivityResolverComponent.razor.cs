using System.Reflection;
using Geometrician.Core.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Composition;

public class ActivityResolverComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ActivityResolverComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="CompositionState"/>
    [Inject]
    private CompositionState Resolver { get; set; } = null!;

    /// <inheritdoc cref="IActivityFactory"/>
    [Inject]
    private IActivityFactory Factory { get; set; } = default!;

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
        Logger.LogDebug($"{nameof(OnInitialized)} called. Activity: {Resolver.SelectedActivityKey} Assembly: {Resolver.SelectedActivityKey}");
        if (!Resolver.TryGetAssemblyByKey(Resolver.SelectedAssemblyKey, out Assembly? assembly))
            return;
        ActivitySelectOptions = Factory.AllActivityKeysInAssembly(assembly!).ToArray();
        if (ActivitySelectOptions.Any())
            ActivitySelectValue = ActivitySelectOptions.First();
    }

    protected void SetActivity()
    {
        Logger.LogDebug($"{nameof(SetActivity)} called with {ActivitySelectValue}.");
        Navigation.NavigateTo($"/run/{Resolver.SelectedAssemblyKey}/{ActivitySelectValue}");
    }
}
