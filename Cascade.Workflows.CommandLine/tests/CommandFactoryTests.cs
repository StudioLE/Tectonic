using System.CommandLine;
using Cascade.Workflows.CommandLine.Tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;
using StudioLE.Verify.Yaml;

namespace Cascade.Workflows.CommandLine.Tests;

internal sealed class CommandFactoryTests
{
    internal const int ExpectedArgumentsCount = 0;
    internal const int ExpectedOptionsCount = 6;
    internal const int ExpectedChildrenCount = ExpectedArgumentsCount + ExpectedOptionsCount;
    private readonly IVerify _verify = new NUnitVerify();

    [Test]
    public async Task CommandFactory_Build()
    {
        // Arrange
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddTransient<ExampleActivity>();
                services.AddTransient<CommandFactory>();
                services.AddTransient<IIsParseableStrategy, IsParseableStrategy>();
                services.AddTransient<ICommandOptionsStrategy, CommandOptionsStrategy>();
                services.AddTransient<ICommandHandlerStrategy, CommandHandlerStrategy>();
            })
            .Build();
        CommandFactory factory = host.Services.GetRequiredService<CommandFactory>();
        IActivity activity = host.Services.GetRequiredService<ExampleActivity>();

        // Act
        Command command = factory.Create(activity);

        // Assert
        await _verify.AsYaml(command
            .Options
            .Select(x => new
            {
                x.Description,
                Type = x.ValueType.Name,
                x.Aliases
            }));
        Assert.Multiple(() =>
        {
            Assert.That(command.Children.Count(), Is.EqualTo(ExpectedChildrenCount));
            Assert.That(command.Options.Count, Is.EqualTo(ExpectedOptionsCount));
            Assert.That(command.Arguments.Count, Is.EqualTo(ExpectedArgumentsCount));
        });
    }
}
