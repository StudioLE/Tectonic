using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Cascade.Components.Shared;

/// <summary>
/// Methods for displaying messages in the UI.
/// </summary>
public class CommunicationState
{
    private readonly ISnackbar _snackbar;

    /// <inheritdoc cref="CommunicationState"/>
    public CommunicationState(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    /// <summary>
    /// Show a message in the UI.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Show(Message message)
    {
        Severity severity = message.Level switch
        {
            LogLevel.Trace => Severity.Normal,
            LogLevel.Debug => Severity.Normal,
            LogLevel.Information => Severity.Info,
            LogLevel.Warning => Severity.Warning,
            LogLevel.Error => Severity.Error,
            LogLevel.Critical => Severity.Error,
            LogLevel.None => Severity.Normal,
            _ => Severity.Normal
        };
        _snackbar.Add(message.Body, severity);
    }

    /// <summary>
    /// Show an error message in the UI.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ShowError(string message)
    {
        _snackbar.Add(message, Severity.Error);
    }

    /// <summary>
    /// Show a warning message in the UI.
    /// </summary>
    /// <param name="message">The message.</param>
    public void ShowWarning(string message)
    {
        _snackbar.Add(message, Severity.Warning);
    }
}
