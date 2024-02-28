using System.CommandLine;
using Cascade.Workflows.CommandLine.Tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify.Yaml;

namespace Cascade.Workflows.CommandLine.Tests;

internal sealed class CommandFactoryTests
{
    internal const int ExpectedArgumentsCount = 2;
    internal const int ExpectedOptionsCount = 9;
    internal const int ExpectedChildrenCount = ExpectedArgumentsCount + ExpectedOptionsCount;
    private readonly IContext _context = new NUnitContext();

    [Test]
    public async Task CommandFactory_Build()
    {
        // Arrange
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services => services
                .AddCommandBuilderServices()
                .AddTransient<ExampleActivity>())
            .Build();

        IServiceScope scope = host.Services.CreateScope();
        CommandFactory factory = scope.ServiceProvider.GetRequiredService<CommandFactory>();
        IActivity activity = scope.ServiceProvider.GetRequiredService<ExampleActivity>();

        // Act
        Command command = factory.Create(activity);

        // Assert
        await _context.VerifyAsYaml(command
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
