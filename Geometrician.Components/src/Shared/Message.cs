using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Shared;

/// <summary>
/// A message to display within the UI.
/// </summary>
public class Message
{
    /// <summary>
    /// The severity of the message.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// An optional title for the message.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// The body of the message.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// <inheritdoc cref="Message"/>
    /// </summary>
    /// <param name="level">The severity of the message.</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="title">An optional title for the message.</param>
    public Message(LogLevel level, string body, string? title = null)
    {
        Level = level;
        Body = body;
        Title = title;
    }
}
