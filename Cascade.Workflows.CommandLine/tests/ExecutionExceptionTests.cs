using System.CommandLine;
using Cascade.Workflows.CommandLine.Logging;
using Cascade.Workflows.CommandLine.Tests.Resources;
using Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Cascade.Workflows.CommandLine.Tests;

internal sealed class ExecutionExceptionTests
{
    private readonly TestLogger _logger = TestLogger.GetInstance();
    private RedirectConsoleToLogger _console = null!;
    private RootCommand _command = null!;

    [SetUp]
    public void SetUp()
    {
        _console = new(_logger);
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureLogging(logging => logging.AddTestLogger())
            .ConfigureServices(services => services
                .AddCommandBuilderServices()
                .AddTransient<ExampleThatThrowsActivity>())
            .Build();
        CommandBuilder builder = host
            .Services
            .GetRequiredService<CommandBuilder>();
        _command = builder
            .Register<ExampleThatThrowsActivity>()
            .Build();
        _command.Name = "RootCommand";
        _logger.Clear();
    }

    [Test]
    public void ExecutionExceptionTests_Throws()
    {
        // Arrange
        string[] arguments =
        {
            // ReSharper disable StringLiteralTypo
            "examplethatthrowsactivity",
            "--integervalue",
            "15",
            "--nested.integervalue",
            "10",
            "--stringvalue",
            "Hello, world!"
            // ReSharper restore StringLiteralTypo
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        Assert.Multiple(() =>
        {
            // TODO: Set the exit code if an exception is thrown?
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Critical), Is.EqualTo(1), "Critical count");
        });
    }
}
