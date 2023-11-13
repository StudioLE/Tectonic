using System.Reflection;
using NUnit.Framework;
using StudioLE.Results;
using StudioLE.Core.System;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Cascade.Workflows.NUnit.Tests;

internal sealed class NUnitActivityResolverTests
{
    private readonly IVerify _verify = new NUnitVerify();
    private const string AssemblyPath = "Cascade.Workflows.NUnit.Samples.dll";
    private const string ActivityKey = "Cascade.Workflows.NUnit.Samples.NUnitTestSamples.NUnitTestSamples_Test_Verify";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    public async Task NUnitActivityResolver_AllActivityKeysInAssembly()
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        string[] activities = resolver.AllActivityKeysInAssembly(_assembly).ToArray();

        // Assert
        await _verify.String(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(4), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void NUnitActivityResolver_Resolve(string activityKey)
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = result.GetValueOrThrow();

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.GetName(), Is.Not.Empty, "Name");
        // Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        // Assert.That(activity.Inputs.Length, Is.EqualTo(0), "Inputs count");
    }
}
