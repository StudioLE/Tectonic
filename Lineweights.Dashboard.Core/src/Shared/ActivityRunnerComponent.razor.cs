using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using Ardalis.Result;
using Lineweights.Flex.Samples;
using Lineweights.Workflows;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Results;
using Lineweights.Workflows.Samples;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StudioLE.Core.System;

namespace Lineweights.Dashboard.Core.Shared;

public class ActivityRunnerComponentBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<ActivityRunnerComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="ActivityBuilder"/>
    [Inject]
    protected ActivityBuilder Builder { get; set; } = default!;

    /// <inheritdoc cref="ResultsState"/>
    [Inject]
    protected ResultsState Results { get; set; } = default!;

    protected string AssemblyInputValue { get; set; } = string.Empty;

    protected Dictionary<string, Assembly> AssemblySelectOptions { get; } = new();

    protected string AssemblySelectValue { get; set; } = string.Empty;

    protected string ActivitySelectValue { get; set; } = string.Empty;

    protected ObservableCollection<Message> Messages { get; } = new();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Assembly[] assemblies = {
            typeof(DashboardSamples).Assembly,
            typeof(WallFlemishBond).Assembly
        };
        foreach (Assembly assembly in assemblies)
            AssemblySelectOptions.Add(assembly.GetName().Name ?? "Unnamed", assembly);
        AssemblySelectValue = AssemblySelectOptions.First().Key;
        Messages.CollectionChanged += OnMessagesChanged;
    }

    protected void SetInitialState()
    {
        ClearMessages();

        Builder = new();
    }

    protected void SetAssembly(string key)
    {
        ClearMessages();
        Logger.LogDebug($"{nameof(SetAssembly)} called with key {key}.");
        if(!AssemblySelectOptions.TryGetValue(key, out Assembly? assembly))
        {
            ShowWarning("Failed to load assembly. Key not found.");
            return;
        }
        SetAssembly(assembly);
    }

    protected void SetAssembly()
    {
        ClearMessages();
        string assemblyPath = AssemblyInputValue;
        Logger.LogDebug($"{nameof(SetAssembly)} called with {assemblyPath}.");
        if (!Path.IsPathFullyQualified(assemblyPath))
        {
            string assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
            assemblyPath = Path.Combine(assemblyDir, assemblyPath);
        }
        if (!File.Exists(assemblyPath))
        {
            ShowWarning("Failed to load assembly. File not found.");
            return;
        }
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        SetAssembly(assembly);
    }

    private void SetAssembly(Assembly assembly)
    {
        Result<object> result = Builder.SetAssembly(assembly);
        if (!result.IsSuccess)
            ShowError(result.Errors.Join());

        if(Builder.State is ActivityBuilder.AssemblySetState state)
            ActivitySelectValue = state.ActivityNames.First();
    }

    protected void SetActivity()
    {
        ClearMessages();

        Logger.LogDebug($"{nameof(SetActivity)} called with {ActivitySelectValue}.");
        Result<object> result = Builder.SetActivity(ActivitySelectValue);
        if (!result.IsSuccess)
            ShowError(result.Errors.Join());
    }

    protected void BuildAndExecute()
    {
        ClearMessages();

        Logger.LogDebug($"{nameof(BuildAndExecute)} called.");
        Result<object> buildCommandResult = Builder.Build();
        if (!buildCommandResult.IsSuccess)
        {
            ShowWarning(buildCommandResult.Errors.Join());
            return;
        }

        if (Builder.State is not ActivityBuilder.BuiltState builtState)
        {
            Logger.LogWarning("Runner was in an incorrect state.");
            return;
        }

        // Arrange
        ActivityCommand activity = builtState.Command;

        Result<object> executionResult;
        Logger.LogDebug("Execution started.");
        // TODO: Add a timer.
        try
        {
            executionResult = activity.Execute();
        }
        catch (TargetInvocationException e)
        {
            ShowError(e.InnerException ?? e, "Execution failed");
            return;
        }
        catch (Exception e)
        {
            ShowError(e, "Execution failed");
            return;
        }
        Logger.LogDebug("Execution completed.");

        if (!executionResult.IsSuccess)
        {
            ShowWarning(executionResult.Errors.Join());
            return;
        }

        dynamic outputs = executionResult.Value;
        if (outputs.Model is not Model model)
        {
            ShowWarning("Activity output was not a model.");
            return;
        }

        DocumentInformation doc = new()
        {
            Name = builtState.Command.Name,
            Description = $"Executed {ActivitySelectValue} from {AssemblyInputValue}."
        };
        Result result = ResultBuilder.Default(new BlobStorageStrategy(), model, doc);
        Results.Collection.Add(result);
    }

    private void ShowWarning(string message, string? title = null)
    {
        Logger.LogWarning(message);
        Messages.Add(new(LogLevel.Warning, message, title));
    }

    private void ShowError(string message, string? title = null)
    {
        Logger.LogError(message);
        Messages.Add(new(LogLevel.Warning, message, title));
    }

    private void ShowError(Exception e, string? title = null)
    {
        Logger.LogError(e, title);
        Messages.Add(new(LogLevel.Warning, e.Message, title));
    }

    private void ClearMessages()
    {
        Messages.Clear();
    }

    /// <summary>
    /// Notify that the state has changed.
    /// </summary>
    private async void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Logger.LogDebug($"{nameof(OnMessagesChanged)}() called.");
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Results.Collection.CollectionChanged -= OnMessagesChanged;
    }
}
