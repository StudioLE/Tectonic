using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Tectonic.Extensions.CommandLine.Tests.Resources;

namespace Tectonic.Extensions.CommandLine.Tests;

internal sealed class CommandBuilderTests
{
    [Test]
    public void CommandBuilder_Build()
    {
        // Arrange
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services => services
                .AddCommandBuilderServices()
                .AddTransient<ExampleActivity>())
            .Build();
        CommandBuilder builder = host
            .Services
            .GetRequiredService<CommandBuilder>();

        // Act
        RootCommand command = builder
            .Register<ExampleActivity>()
            .Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(command.Options.Count, Is.EqualTo(0));
            Assert.That(command.Subcommands.Count, Is.EqualTo(1));
            Command subCommand = command.Subcommands.First();
            Assert.That(subCommand.Children.Count(), Is.EqualTo(CommandFactoryTests.ExpectedChildrenCount));
            Assert.That(subCommand.Options.Count, Is.EqualTo(CommandFactoryTests.ExpectedOptionsCount));
            Assert.That(subCommand.Arguments.Count, Is.EqualTo(CommandFactoryTests.ExpectedArgumentsCount));
        });
    }

    [Test]
    public void CommandBuilder_Build_Nested()
    {
        // Arrange
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services => services
                .AddCommandBuilderServices()
                .AddTransient<ExampleActivity>())
            .Build();
        CommandBuilder builder = host
            .Services
            .GetRequiredService<CommandBuilder>();

        // Act
        RootCommand command = builder
            .Register<ExampleActivity>("get", "one")
            .Register<ExampleActivity>("get", "two")
            .Register<ExampleActivity>("get", "three")
            .Register<ExampleActivity>("set", "one")
            .Register<ExampleActivity>("update")
            .Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(command.Options.Count, Is.EqualTo(0));
            Assert.That(command.Subcommands.Count, Is.EqualTo(3));
            Command getCommand = command.Subcommands.First(x => x.Name == "get");
            Assert.That(getCommand.Subcommands.Count, Is.EqualTo(3));
            Assert.That(getCommand.Options.Count, Is.EqualTo(0));
            Assert.That(getCommand.Arguments.Count, Is.EqualTo(0));
            Command getOneCommand = getCommand.Subcommands.First(x => x.Name == "one");
            Assert.That(getOneCommand.Options.Count, Is.EqualTo(CommandFactoryTests.ExpectedOptionsCount));
            Assert.That(getOneCommand.Arguments.Count, Is.EqualTo(CommandFactoryTests.ExpectedArgumentsCount));
            Command setCommand = command.Subcommands.First(x => x.Name == "set");
            Assert.That(setCommand.Subcommands.Count, Is.EqualTo(1));
            Assert.That(setCommand.Options.Count, Is.EqualTo(0));
            Assert.That(setCommand.Arguments.Count, Is.EqualTo(0));
            Command updateCommand = command.Subcommands.First(x => x.Name == "update");
            Assert.That(updateCommand.Subcommands.Count, Is.EqualTo(0));
            Assert.That(updateCommand.Options.Count, Is.EqualTo(CommandFactoryTests.ExpectedOptionsCount));
            Assert.That(updateCommand.Arguments.Count, Is.EqualTo(CommandFactoryTests.ExpectedArgumentsCount));
        });
    }
}
