using System.Collections.ObjectModel;
using Geometrician.Components.Shared;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Composition;

/// <summary>
/// The current state of the activity composition.
/// </summary>
public class CompositionState
{
    /// <summary>
    /// The messages to display in the UI.
    /// </summary>
    public ObservableCollection<Message> Messages { get; } = new();

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
