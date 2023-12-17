using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Cascade.Workflows.CommandLine.Logging;
using Cascade.Workflows.CommandLine.Tests.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.System;
using StudioLE.Verify;

namespace Cascade.Workflows.CommandLine.Tests;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal sealed class ExecutionTests
{
    private readonly IContext _context = new NUnitContext();
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
                .AddTransient<ExampleActivity>()
                .AddTransient<ExampleErrorActivity>())
            .Build();
        _services = host.Services;
        ILogger logger = _services.GetRequiredService<ILogger<ExecutionTests>>();
        _console = new(logger);
        CommandBuilder builder = host
            .Services
            .GetRequiredService<CommandBuilder>();
        _command = builder
            .Register<ExampleActivity>()
            .Register<ExampleErrorActivity>()
            .Build();
        _command.Name = "RootCommand";
    }

    [Test]
    public async Task ExecutionTests_No_Command()
    {
        // Arrange
        string[] arguments =
        {
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(2), "Error count");
        });
        await _context.Verify(logs);
    }

    [Test]
    public void ExecutionTests_Command_LowerCase()
    {
        // Arrange
        string[] arguments =
        {
            "exampleactivity"
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
    }

    [Test]
    public void ExecutionTests_Command_CamelCase()
    {
        // Arrange
        string[] arguments =
        {
            "ExampleActivity"
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(3), "Error count");
        });
    }

    [Test]
    public void ExecutionTests_Invalid_Argument()
    {
        // Arrange
        string[] arguments =
        {
            "exampleactivity",
            "--nope",
            "1"
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(async () =>
        {
            await _context.Verify(logs);
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(3), "Error count");
        });
    }

    [Test]
    public void ExecutionTests_Validation_Errors()
    {
        // Arrange
        string[] arguments =
        {
            // ReSharper disable StringLiteralTypo
            "exampleactivity",
            "--integervalue",
            "1",
            "--nested.integervalue",
            "50",
            "--stringvalue",
            ""
            // ReSharper restore StringLiteralTypo
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(async () =>
        {
            await _context.Verify(logs);
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(4), "Error count");
        });
    }

    [Test]
    public void ExecutionTests_Validation_No_Errors()
    {
        // Arrange
        string[] arguments =
        {
            // ReSharper disable StringLiteralTypo
            "exampleactivity",
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
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
    }

    [Test]
    public async Task ExecutionTests_Options_BooleanValue_NotSet()
    {
        // Arrange
        string[] arguments =
        {
            "exampleactivity"
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
        string output = logs.Where(x => x.LogLevel == LogLevel.Information).Select(x => x.Message).Join();
        await _context.Verify(output);
    }

    [Test]
    public async Task ExecutionTests_Options_BooleanValue_Set()
    {
        // Arrange
        string[] arguments =
        {
            "exampleactivity",
            "--booleanvalue",
            "--nested.booleanvalue"
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
        string output = logs.Where(x => x.LogLevel == LogLevel.Information).Select(x => x.Message).Join();
        await _context.Verify(output);
    }

    [Test]
    public void ExecutionTests_Command_ExitCode()
    {
        // Arrange
        string[] arguments =
        {
            "exampleerroractivity"
        };

        // Act
        int exitCode = _command.Invoke(arguments, _console);

        // Assert
        _console.Flush();
        IReadOnlyCollection<LogEntry> logs = _services.GetCachedLogs();
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(99), "Exit code");
            Assert.That(logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
    }
}
