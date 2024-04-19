using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StudioLE.Extensions.Logging.Cache;
using Tectonic.Core.Samples.ClassExample;
using Tectonic.Core.Samples.StructExample;
using ExampleEnum = Tectonic.Core.Samples.ClassExample.ExampleEnum;

namespace Tectonic.Core.Tests;

internal sealed class ActivityTests
{
    private const string ClassActivityKey = "class-activity";
    private const string StructActivityKey = "struct-activity";
    private ActivityProvider _provider;
    private IReadOnlyCollection<LogEntry> _logs;

    [SetUp]
    public void SetUp()
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging => logging
                .AddConsole()
                .AddCache())
            .ConfigureServices(services => services
                .AddTransient<ActivityUsingClasses>()
                .AddTransient<ActivityUsingStructs>())
            .Build();
        _provider = new ActivityProviderBuilder(host.Services)
            .Add<ActivityUsingClasses>(ClassActivityKey)
            .Add<ActivityUsingStructs>(StructActivityKey)
            .Build();
        _logs = host.Services.GetCachedLogs();
    }

    [Test]
    public async Task Activity_ExecuteNonGeneric_ClassBased()
    {
        // Arrange
        IActivity activity = _provider.Get(ClassActivityKey) ?? throw new("Failed to get activity.");
        InputsClass inputs = new()
        {
            EnumValue = ExampleEnum.Three,
            Nested = new()
            {
                StringValue = "This is a nested string value string",
                IntegerValue = 15,
                DoubleValue = 0.5
            }
        };

        // Act
        object? result = await activity.ExecuteNonGeneric(inputs);

        // Assert
        Assert.That(result, Is.Not.Null, "Output");
        Assert.That(result, Is.InstanceOf<OutputsClass>(), "Output type");
        if (result is OutputsClass outputs)
        {
            Assert.That(outputs.EnumValue, Is.EqualTo(inputs.EnumValue), "Outputs enum");
            Assert.That(outputs.Nested, Is.EqualTo(inputs.Nested), "Outputs nested class");
        }
        Assert.That(_logs.Count, Is.EqualTo(1), "Log count");
    }

    [Test]
    public async Task Activity_ExecuteNonGeneric_StructBased()
    {
        // Arrange
        IActivity activity = _provider.Get(StructActivityKey) ?? throw new("Failed to get activity.");
        InputsStruct inputs = new()
        {
            EnumValue = ExampleEnum.Three,
            Nested = new()
            {
                StringValue = "This is a nested string value string",
                IntegerValue = 15,
                DoubleValue = 0.5
            }
        };

        // Act
        object? result = await activity.ExecuteNonGeneric(inputs);

        // Assert
        Assert.That(result, Is.Not.Null, "Output");
        Assert.That(result, Is.InstanceOf<OutputsStruct>(), "Output type");
        if (result is OutputsStruct outputs)
        {
            Assert.That(outputs.EnumValue, Is.EqualTo(inputs.EnumValue), "Outputs enum");
            Assert.That(outputs.Nested, Is.EqualTo(inputs.Nested), "Outputs nested");
        }
        Assert.That(_logs.Count, Is.EqualTo(1), "Log count");
    }
}
