﻿using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.App.Core.Shared;

public class AssemblySelectionComponentBase : ComponentBase
{
    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    public ILogger<AssemblySelectionComponent> Logger { get; set; } = null!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc cref="RunnerState"/>
    [Inject]
    public RunnerState State { get; set; } = null!;

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
        AssemblySelectOptions = State.LoadedAssemblies.Keys.ToArray();
        if(AssemblySelectOptions.Any())
            AssemblySelectValue = AssemblySelectOptions.First();
    }

    protected void SetAssemblyByKey()
    {
        Logger.LogDebug($"{nameof(SetAssemblyByKey)} called. Activity: {State.SelectedActivityKey} Assembly: {State.SelectedActivityKey}");
        if(!State.TryGetAssemblyByKey(AssemblySelectValue, out Assembly? assembly))
            return;
        AssemblyInputValue = $"{AssemblySelectValue}.dll";
        SetAssembly(assembly!);
    }

    protected void SetAssemblyByPath()
    {
        State.Messages.Clear();
        Logger.LogDebug($"{nameof(SetAssemblyByPath)} called with {AssemblyInputValue}.");
        string assemblyPath = AssemblyInputValue;
        if (!Path.IsPathFullyQualified(assemblyPath))
        {
            string assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
            assemblyPath = Path.Combine(assemblyDir, assemblyPath);
        }
        if (!File.Exists(assemblyPath))
        {
            State.ShowWarning(Logger, "Failed to load assembly. File not found.");
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
            State.ShowWarning(Logger, "Failed to load assembly. Assembly doesn't have a name.");
            return;
        }
        _ = State.LoadedAssemblies.TryAdd(assemblyName, assembly);
        Navigation.NavigateTo($"/run/{assemblyName}");
    }
}
