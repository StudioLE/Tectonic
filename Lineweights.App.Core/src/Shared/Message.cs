using Microsoft.Extensions.Logging;

namespace Lineweights.App.Core.Shared;

public class Message
{
    public LogLevel Level { get; set; }

    public string? Title { get; set; }

    public string Body { get; set; }

    public Message(LogLevel level, string body, string? title = null)
    {
        Level = level;
        Body = body;
        Title = title;
    }

    public string CssClass()
    {
        return Level switch
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
}
