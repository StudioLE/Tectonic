using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Shared;

/// <summary>
/// A <see cref="IComponent"/> to render a <see cref="Message"/> in the UI.
/// </summary>
public class MessageComponentBase : ComponentBase
{
    /// <summary>
    /// The message.
    /// </summary>
    [Parameter]
    public Message Message { get; set; } = default!;

    /// <summary>
    /// The css class based on the message severity.
    /// </summary>
    protected string Class => Message.Level switch
    {
        LogLevel.Trace => "is-dark",
        LogLevel.Debug => "is-dark",
        LogLevel.Information => "is-info",
        LogLevel.Warning => "is-warning",
        LogLevel.Error => "is-danger",
        LogLevel.Critical => "is-danger",
        LogLevel.None => "is-dark",
        _ => "is-dark"
    };
}
