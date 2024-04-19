using Microsoft.Extensions.Logging;

namespace Tectonic.Core.Samples.StructExample;

public class ActivityUsingStructs : ActivityBase<InputsStruct, OutputsStruct>
{
    private readonly ILogger<ActivityUsingStructs> _logger;

    public ActivityUsingStructs(ILogger<ActivityUsingStructs> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public override Task<OutputsStruct?> Execute(InputsStruct inputs)
    {
        _logger.LogInformation("Executed ExampleActivity");
        OutputsStruct outputs = new()
        {
            EnumValue = inputs.EnumValue,
            Nested = inputs.Nested
        };
        return Task.FromResult<OutputsStruct?>(outputs);
    }
}
