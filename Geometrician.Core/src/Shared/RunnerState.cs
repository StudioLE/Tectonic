using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Shared;

public class RunnerState
{
    private readonly ILogger<RunnerState> _logger;

    public RunnerState(ILogger<RunnerState> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The inputs for the activity.
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
    public bool TryGetAssemblyByKey(string key, out Assembly? assembly)
    {
        _logger.LogDebug($"{nameof(TryGetAssemblyByKey)} called with key {key}.");
        if(!LoadedAssemblies.TryGetValue(key, out assembly))
        {
            ShowError(_logger, "Failed to load assembly. Key not found.");
            return false;
        }
        return true;
    }

    public void ShowWarning(ILogger logger, string message, string? title = null)
    {
        logger.LogWarning(message);
        Messages.Add(new(LogLevel.Warning, message, title));
    }

    public void ShowError(ILogger logger, string message, string? title = null)
    {
        logger.LogError(message);
        Messages.Add(new(LogLevel.Error, message, title));
    }

    public void ShowError(ILogger logger, Exception e, string? title = null)
    {
        logger.LogError(e, title);
        Messages.Add(new(LogLevel.Error, e.Message, title));
    }

}
