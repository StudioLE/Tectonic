using Microsoft.Extensions.Logging;

namespace Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;

[ProviderAlias("Test")]
public class TestLoggerProvider : ILoggerProvider
{
    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        return TestLogger.GetInstance();
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
