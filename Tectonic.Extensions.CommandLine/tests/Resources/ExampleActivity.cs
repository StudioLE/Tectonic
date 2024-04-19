using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public class ExampleActivity : ActivityBase<ExampleClass, ExampleClass>
{
    private readonly ILogger<ExampleActivity> _logger;

    public ExampleActivity(ILogger<ExampleActivity> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public override Task<ExampleClass?> Execute(ExampleClass example)
    {
        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        string yaml = serializer.Serialize(example);
        _logger.LogInformation(yaml);
        return Task.FromResult<ExampleClass?>(example);
    }
}
