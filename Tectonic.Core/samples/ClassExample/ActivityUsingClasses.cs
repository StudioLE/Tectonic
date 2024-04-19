using Microsoft.Extensions.Logging;

namespace Tectonic.Core.Samples.ClassExample;

public class ActivityUsingClasses : ActivityBase<InputsClass, OutputsClass>
{
    private readonly ILogger<ActivityUsingClasses> _logger;

    public ActivityUsingClasses(ILogger<ActivityUsingClasses> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public override Task<OutputsClass?> Execute(InputsClass inputs)
    {
        _logger.LogInformation("Executed ExampleActivity");
        OutputsClass outputs = new()
        {
            EnumValue = inputs.EnumValue,
            Nested = inputs.Nested
        };
        return Task.FromResult<OutputsClass?>(outputs);
    }
}
