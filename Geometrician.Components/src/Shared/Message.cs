using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Shared;

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
}
