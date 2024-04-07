using StudioLE.Diagnostics;
using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.System;
using StudioLE.Verify;

namespace Tectonic.Extensions.CommandLine.Tests;

public static class VerifyExtensions
{
    public static Task Verify(this IContext context, IEnumerable<LogEntry> logs)
    {
        string value = logs
            .Select(x => x.Message)
            .Join();
        return context.Verify(value);
    }
}
