using Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;
using StudioLE.Extensions.System;
using StudioLE.Verify;

namespace Cascade.Workflows.CommandLine.Tests;

public static class VerifyExtensions
{
    public static Task AsString(this IVerify verify, TestLogger logger)
    {
        string value = logger
            .Logs
            .Select(x => x.Message)
            .Join();
        return verify.String(value);
    }
}
