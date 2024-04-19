using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Extensions.Logging.Cache;
using StudioLE.Extensions.System;
using StudioLE.Verify;
using Tectonic.Core.Samples.ClassExample;
using Tectonic.Core.Samples.StructExample;

namespace Tectonic.Core.Tests;

internal sealed class ActivityProviderTests
{
    private const string ClassActivityKey = "class-activity";
    private const string StructActivityKey = "struct-activity";
    private readonly IContext _context = new NUnitContext();
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
    public async Task ActivityProvider_GetKeys()
    {
        // Arrange
        // Act
        string[] activities = _provider.GetKeys().ToArray();

        // Assert
        await _context.Verify(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(2), "Activity count");
        Assert.That(_logs.Count, Is.EqualTo(0), "Log count");
    }

    [Test]
    public void ActivityProvider_Get()
    {
        // Arrange
        // Act
        IActivity? activity = _provider.Get(ClassActivityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
        Assert.That(activity, Is.InstanceOf<ActivityUsingClasses>());
        Assert.That(_logs.Count, Is.EqualTo(0), "Log count");
    }
}
