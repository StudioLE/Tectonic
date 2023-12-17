using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.System;
using StudioLE.Diagnostics;
using StudioLE.Verify;

namespace Cascade.Workflows.CommandLine.Tests;

public static class VerifyExtensions
{
    public static Task Verify(this IContext context, IReadOnlyCollection<LogEntry> logs)
    {
        string value = logs
            .Select(x => x.Message)
            .Join();
        return context.Verify(value);
    }
}
