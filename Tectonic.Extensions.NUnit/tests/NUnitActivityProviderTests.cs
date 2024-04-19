using System.Reflection;
using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Extensions.System;
using StudioLE.Extensions.System.Reflection;
using StudioLE.Verify;

namespace Tectonic.Extensions.NUnit.Tests;

internal sealed class NUnitActivityProviderTests
{
    private const string AssemblyPath = "Tectonic.Extensions.NUnit.Samples.dll";
    private const string ActivityKey = "Tectonic.Extensions.NUnit.Samples.NUnitTestSamples.NUnitTestSamples_Test_Verify";
    private readonly IContext _context = new NUnitContext();
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);
    private NUnitActivityProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _provider = new NUnitActivityProviderBuilder()
            .Add(_assembly)
            .Build();
    }

    [Test]
    public async Task NUnitActivityProvider_GetKeys()
    {
        // Arrange

        // Act
        string[] activities = _provider.GetKeys().ToArray();

        // Assert
        await _context.Verify(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(4), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void NUnitActivityProvider_Get(string activityKey)
    {
        // Arrange
        // Act
        IActivity? activity = _provider.Get(activityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
        Assert.That(activity, Is.InstanceOf<NUnitActivity>());
    }
}
