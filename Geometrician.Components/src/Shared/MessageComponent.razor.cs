using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Shared;

public class MessageComponentBase : ComponentBase
{
    /// <summary>
    /// The message.
    /// </summary>
    [Parameter]
    public Message Message { get; set; } = default!;

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
