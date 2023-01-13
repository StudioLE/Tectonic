using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Composition;

/// <summary>
/// A <see cref="IComponent"/> to select an assembly to load activities from.
/// </summary>
public class AssemblyResolverComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<AssemblyResolverComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="CompositionState"/>
    [Inject]
    private CompositionState Resolver { get; set; } = null!;

    /// <summary>
    /// The error messages.
    /// </summary>
    protected string[] Errors { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Are the form inputs valid?
    /// </summary>
    protected bool IsValid { get; set; }

    /// <summary>
    /// Binding for the assembly text input element value.
    /// </summary>
    protected string AssemblyInputValue { get; set; } = string.Empty;

    /// <summary>
    /// Binding for the options of the assembly select element.
    /// </summary>
    protected IReadOnlyCollection<string> AssemblySelectOptions { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Binding for the assembly select element value.
    /// </summary>
    protected string AssemblySelectValue { get; set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        AssemblySelectOptions = Resolver.LoadedAssemblies.Keys.ToArray();
        if (AssemblySelectOptions.Any())
            AssemblySelectValue = AssemblySelectOptions.First();
    }

    /// <summary>
    /// Set the assembly by key.
    /// </summary>
    protected void SetAssemblyByKey()
    {
        Logger.LogDebug($"{nameof(SetAssemblyByKey)} called. Activity: {Resolver.SelectedActivityKey} Assembly: {Resolver.SelectedAssemblyKey}");
        if (!Resolver.TryGetAssemblyByKey(AssemblySelectValue, out Assembly? assembly))
            return;
        AssemblyInputValue = $"{AssemblySelectValue}.dll";
        SetAssembly(assembly!);
    }

    /// <summary>
    /// Set the assembly by path.
    /// </summary>
    protected void SetAssemblyByPath()
    {
        Resolver.Messages.Clear();
        Logger.LogDebug($"{nameof(SetAssemblyByPath)} called with {AssemblyInputValue}.");
        string assemblyPath = AssemblyInputValue;
        if (!Path.IsPathFullyQualified(assemblyPath))
        {
            string assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
            assemblyPath = Path.Combine(assemblyDir, assemblyPath);
        }
        if (!File.Exists(assemblyPath))
        {
            Resolver.ShowWarning(Logger, "Failed to load assembly. File not found.");
            return;
        }
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        SetAssembly(assembly);
    }

    private void SetAssembly(Assembly assembly)
    {
        string? assemblyName = assembly.GetName().Name;
        if (assemblyName is null)
        {
            Resolver.ShowWarning(Logger, "Failed to load assembly. Assembly doesn't have a name.");
            return;
        }
        _ = Resolver.LoadedAssemblies.TryAdd(assemblyName, assembly);
        Navigation.NavigateTo($"/run/{assemblyName}");
    }
}
