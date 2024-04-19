using System.Reflection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.System.Reflection;
using Tectonic.Core.Samples.StaticExample;
using Tectonic.StaticMethodActivities;

namespace Tectonic.Core.Tests.StaticMethodActivities;

internal sealed class StaticMethodActivityTests
{
    private const string AssemblyPath = "Tectonic.Core.Samples.dll";
    private const string ActivityKey = "Tectonic.Core.Samples.StaticExample.StaticMethodActivityExample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);
    private CacheLoggerProvider _cache;
    private ILoggerFactory _loggerFactory;
    private StaticMethodActivityProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _cache = new();
        _loggerFactory = new LoggerFactory(new[] { _cache });
        _provider = new StaticMethodActivityProviderBuilder(_loggerFactory)
            .Add(_assembly)
            .Build();
    }


    [Test]
    public void StaticMethodActivity_InputType()
    {
        // Arrange
        IActivity activity = _provider.Get(ActivityKey) ?? throw new("Failed to get activity.");

        // Act
        Type inputType = activity.InputType;

        // Assert
        Assert.That(inputType.FullName, Is.EqualTo("Tectonic.Core.Samples.StaticExample.StaticMethodActivityExample+Inputs"));
    }


    [Test]
    public void StaticMethodActivity_OutputType()
    {
        // Arrange
        IActivity activity = _provider.Get(ActivityKey) ?? throw new("Failed to get activity.");

        // Act
        Type inputType = activity.OutputType;

        // Assert
        Assert.That(inputType.FullName, Is.EqualTo("Tectonic.Core.Samples.StaticExample.StaticMethodActivityExample+Outputs"));
    }

    [Test]
    public async Task StaticMethodActivity_Execute()
    {
        // Arrange
        IActivity activity = _provider.Get(ActivityKey) ?? throw new("Failed to get activity.");
        Type inputType = activity.InputType;
        object inputs = Activator.CreateInstance(inputType) ?? throw new("Failed to create inputs.");

        // Act
        object? result = await activity.ExecuteNonGeneric(inputs);

        // Assert
        Assert.That(result, Is.Not.Null, "Output");
        Assert.That(result!.GetType().FullName, Is.EqualTo("Tectonic.Core.Samples.StaticExample.StaticMethodActivityExample+Outputs"), "Output type");
        if (result is StaticMethodActivityExample.Outputs outputs)
            Assert.That(outputs.IsValid, Is.True, "Output property value");
    }
}
