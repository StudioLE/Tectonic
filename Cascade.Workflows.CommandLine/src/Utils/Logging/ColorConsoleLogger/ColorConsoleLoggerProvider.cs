using Microsoft.Extensions.Logging;

namespace Cascade.Workflows.CommandLine.Utils.Logging.ColorConsoleLogger;

[ProviderAlias("ColorConsole")]
public sealed class ColorConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new ColorConsoleLogger();
    }

    public void Dispose()
    {
    }
}
