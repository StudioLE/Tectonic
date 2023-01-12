using System.IO;
using System.Reflection;
using Geometrician.Components.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Execution;

public class AssemblySelectionComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<AssemblySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="ExecutionState"/>
    [Inject]
    private ExecutionState Execution { get; set; } = null!;

    /// <inheritdoc cref="DisplayState"/>
    [Inject]
    private DisplayState Display { get; set; } = null!;

    protected string[] Errors { get; set; } = Array.Empty<string>();

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
        AssemblySelectOptions = Execution.LoadedAssemblies.Keys.ToArray();
        if (AssemblySelectOptions.Any())
            AssemblySelectValue = AssemblySelectOptions.First();
    }

    protected void SetAssemblyByKey()
    {
        Logger.LogDebug($"{nameof(SetAssemblyByKey)} called. Activity: {Execution.SelectedActivityKey} Assembly: {Execution.SelectedActivityKey}");
        if (!Execution.TryGetAssemblyByKey(AssemblySelectValue, out Assembly? assembly))
            return;
        AssemblyInputValue = $"{AssemblySelectValue}.dll";
        SetAssembly(assembly!);
    }

    protected void SetAssemblyByPath()
    {
        Execution.Messages.Clear();
        Logger.LogDebug($"{nameof(SetAssemblyByPath)} called with {AssemblyInputValue}.");
        string assemblyPath = AssemblyInputValue;
        if (!Path.IsPathFullyQualified(assemblyPath))
        {
            string assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
            assemblyPath = Path.Combine(assemblyDir, assemblyPath);
        }
        if (!File.Exists(assemblyPath))
        {
            Execution.ShowWarning(Logger, "Failed to load assembly. File not found.");
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
            Execution.ShowWarning(Logger, "Failed to load assembly. Assembly doesn't have a name.");
            return;
        }
        _ = Execution.LoadedAssemblies.TryAdd(assemblyName, assembly);
        Navigation.NavigateTo($"/run/{assemblyName}");
    }
}
