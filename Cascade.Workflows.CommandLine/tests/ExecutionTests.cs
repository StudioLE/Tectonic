using System.CommandLine;
using Cascade.Workflows.CommandLine.Logging;
using Cascade.Workflows.CommandLine.Tests.Resources;
using Cascade.Workflows.CommandLine.Utils.Logging.TestLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StudioLE.Core.System;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Cascade.Workflows.CommandLine.Tests;

internal sealed class ExecutionTests
{
    private readonly IVerify _verify = new NUnitVerify();
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
                .AddTransient<ExampleActivity>())
            .Build();
        CommandBuilder builder = host
            .Services
            .GetRequiredService<CommandBuilder>();
        _command = builder
            .Register<ExampleActivity>()
            .Build();
        _command.Name = "RootCommand";
        _logger.Clear();
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
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(2), "Error count");
        });
        await _verify.AsString(_logger);
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
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
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
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(3), "Error count");
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
        Assert.Multiple(async () =>
        {
            await _verify.AsString(_logger);
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(3), "Error count");
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
        Assert.Multiple(async () =>
        {
            await _verify.AsString(_logger);
            Assert.That(exitCode, Is.EqualTo(1), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(4), "Error count");
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
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
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
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
        string output = _logger.Logs.Where(x => x.LogLevel == LogLevel.Information).Select(x => x.Message).Join();
        await _verify.String(output);
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
        Assert.Multiple(() =>
        {
            Assert.That(exitCode, Is.EqualTo(0), "Exit code");
            Assert.That(_logger.Logs.Count(x => x.LogLevel == LogLevel.Error), Is.EqualTo(0), "Error count");
        });
        string output = _logger.Logs.Where(x => x.LogLevel == LogLevel.Information).Select(x => x.Message).Join();
        await _verify.String(output);
    }
}
