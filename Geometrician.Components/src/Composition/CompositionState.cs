using System.Collections.ObjectModel;
using System.Reflection;
using Geometrician.Components.Shared;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Composition;

/// <summary>
/// The current state of the activity composition.
/// </summary>
public class CompositionState
{
    private readonly ILogger<CompositionState> _logger;

    /// <inheritdoc cref="CompositionState"/>
    public CompositionState(ILogger<CompositionState> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The messages to display in the UI.
    /// </summary>
    public ObservableCollection<Message> Messages { get; } = new();

    /// <summary>
    /// The assemblies available to load activities from.
    /// </summary>
    public IDictionary<string, Assembly> LoadedAssemblies { get; } = new Dictionary<string, Assembly>();

    /// <summary>
    /// The key of the currently selected assembly.
    /// This is the assembly name without a .dll extension.
    /// </summary>
    public string SelectedAssemblyKey { get; set; } = string.Empty;

    /// <summary>
    /// The key of the currently selected activity.
    /// This is the activity name without an assembly prefix.
    /// </summary>
    public string SelectedActivityKey { get; set; } = string.Empty;

    /// <summary>
    /// Is the assembly set?
    /// </summary>
    public bool IsAssemblySet => !string.IsNullOrEmpty(SelectedAssemblyKey);

    /// <summary>
    /// Is the activity set?
    /// </summary>
    public bool IsActivitySet => IsAssemblySet && !string.IsNullOrEmpty(SelectedActivityKey);

    /// <summary>
    /// Get the assembly by key.
    /// Log an error and show a warning message if the assembly does not exist.
    /// </summary>
    /// <param name="key">The assembly key.</param>
    /// <param name="assembly">The assembly if successful, otherwise null.</param>
    /// <returns>True if successful, otherwise false.</returns>
    public bool TryGetAssemblyByKey(string key, out Assembly? assembly)
    {
        _logger.LogDebug($"{nameof(TryGetAssemblyByKey)} called with key {key}.");
        if (!LoadedAssemblies.TryGetValue(key, out assembly))
        {
            ShowError(_logger, "Failed to load assembly. Key not found.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Log a warning and show it in the UI.
    /// </summary>
    /// <param name="logger">The logging instance.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="title">An optional title for the log message.</param>
    public void ShowWarning(ILogger logger, string message, string? title = null)
    {
        logger.LogWarning(message);
        Messages.Add(new(LogLevel.Warning, message, title));
    }

    /// <summary>
    /// Log an error and show it in the UI.
    /// </summary>
    /// <param name="logger">The logging instance.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="title">An optional title for the log message.</param>
    public void ShowError(ILogger logger, string message, string? title = null)
    {
        logger.LogError(message);
        Messages.Add(new(LogLevel.Error, message, title));
    }

    /// <summary>
    /// Log an error and show it in the UI.
    /// </summary>
    /// <param name="logger">The logging instance.</param>
    /// <param name="e">The exception to obtain a message from.</param>
    /// <param name="title">An optional title for the log message.</param>
    public void ShowError(ILogger logger, Exception e, string? title = null)
    {
        logger.LogError(e, title);
        Messages.Add(new(LogLevel.Error, e.Message, title));
    }

}
