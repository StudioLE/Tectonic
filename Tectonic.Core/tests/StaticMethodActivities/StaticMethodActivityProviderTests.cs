using System.Reflection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.System;
using StudioLE.Extensions.System.Reflection;
using StudioLE.Verify;
using Tectonic.StaticMethodActivities;

namespace Tectonic.Core.Tests.StaticMethodActivities;

internal sealed class StaticMethodActivityProviderTests
{
    private const string AssemblyPath = "Tectonic.Core.Samples.dll";
    private const string ActivityKey = "Tectonic.Core.Samples.StaticExample.StaticMethodActivityExample.Execute";
    private readonly IContext _context = new NUnitContext();
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
    public async Task StaticMethodActivityProvider_GetKeys()
    {
        // Arrange

        // Act
        string[] activities = _provider.GetKeys().ToArray();

        // Assert
        await _context.Verify(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(1), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void StaticMethodActivityProvider_Get(string activityKey)
    {
        // Arrange
        // Act
        IActivity? activity = _provider.Get(activityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
        Assert.That(activity, Is.InstanceOf<StaticMethodActivity>());
    }
}
