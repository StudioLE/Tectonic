using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StudioLE.Core.System;

namespace Lineweights.Dashboard.Core.Shared;

public class ActivityRunnerComponentBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<ActivityRunnerComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="BasicActivityBuilder"/>
    [Inject]
    protected BasicActivityBuilder Builder { get; set; } = default!;

    /// <inheritdoc cref="ResultsState"/>
    [Inject]
    protected ResultsState Results { get; set; } = default!;

    protected string AssemblyPathInput { get; set; } = "E:/Repos/Hypar/Lineweights/Lineweights.Flex/samples/bin/Debug/netstandard2.0/Lineweights.Flex.Samples.dll";

    protected string ActivityNameSelect { get; set; } = string.Empty;

    protected ObservableCollection<Message> Messages { get; set; } = new();

    protected void SetInitialState()
    {
        ClearMessages();

        Builder = new();
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Messages.CollectionChanged += OnMessagesChanged;
    }

    protected void SetAssembly()
    {
        ClearMessages();

        // TODO: Select box of loaded assemblies.

        Logger.LogDebug($"{nameof(SetAssembly)} called with {AssemblyPathInput}.");
        if (!File.Exists(AssemblyPathInput))
        {
            ShowWarning("Failed to load assembly. File not found.");
            return;
        }

        Assembly assembly = Assembly.LoadFrom(AssemblyPathInput);

        Result<object> result = Builder.SetAssembly(assembly);
        if (!result.IsSuccess)
            ShowError(result.Errors.Join());
    }

    protected void SetActivity()
    {
        ClearMessages();

        Logger.LogDebug($"{nameof(SetActivity)} called with {ActivityNameSelect}.");
        Result<object> result = Builder.SetActivity(ActivityNameSelect);
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
            Description = $"Executed {ActivityNameSelect} from {AssemblyPathInput}."
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
