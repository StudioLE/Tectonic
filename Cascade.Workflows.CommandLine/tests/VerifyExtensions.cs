using Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;
using StudioLE.Core.System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Cascade.Workflows.CommandLine.Tests;

public static class VerifyExtensions
{
    public static Task AsYaml(this StudioLE.Verify.Verify verify, object obj)
    {
        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        string yaml = serializer.Serialize(obj);
        return verify.String(yaml);
    }

    public static Task AsString(this StudioLE.Verify.Verify verify, TestLogger logger)
    {
        string value = logger
            .Logs
            .Select(x => x.Message)
            .Join();
        return verify.String(value);
    }
}
