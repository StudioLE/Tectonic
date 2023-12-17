using System.CommandLine;
using Cascade.Workflows.CommandLine.Logging;
using Cascade.Workflows.CommandLine.Tests.Resources;
using StudioLE.Extensions.Logging.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Cascade.Workflows.CommandLine.Tests;

internal sealed class ExecutionExceptionTests
{
    private RedirectConsoleToLogger _console = null!;
    private RootCommand _command = null!;
    private IServiceProvider _services = null!;

    [SetUp]
    public void SetUp()
    {
        IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureLogging(logging => logging.AddCache())
            .ConfigureServices(services => services
                .AddCommandBuilderServices()
                .AddTransient<ExampleThatThrowsActivity>())
            .Build();
        _services = host.Services;
        ILogger logger = _services.GetRequiredService<ILogger<ExecutionExceptionTests>>();
        _console = new(logger);
        CommandBuilder builder = host
            .Services
            .GetRequiredService<CommandBuilder>();
        _command = builder
            .Register<ExampleThatThrowsActivity>()
            .Build();
        _command.Name = "RootCommand";
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

        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            // TODO: Set the exit code if an exception is thrown?
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Critical), Is.EqualTo(1), "Critical count");
        });
    }
}
