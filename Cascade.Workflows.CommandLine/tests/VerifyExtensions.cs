using Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;
using StudioLE.Extensions.System;
using StudioLE.Diagnostics;
using StudioLE.Verify;

namespace Cascade.Workflows.CommandLine.Tests;

public static class VerifyExtensions
{
    public static Task Verify(this IContext context, TestLogger logger)
    {
        string value = logger
            .Logs
            .Select(x => x.Message)
            .Join();
        return context.Verify(value);
    }
}
