using System.Reflection;
using NUnit.Framework;
using StudioLE.Core.Results;
using StudioLE.Core.System;
using StudioLE.Verify.NUnit;
using StudioLE.Workflows.Abstractions;
using StudioLE.Workflows.NUnit.Execution;

namespace StudioLE.Workflows.NUnit.Tests.Execution;

internal sealed class NUnitActivityResolverTests
{
    private readonly Verify.Verify _verify = new(new NUnitVerifyContext());
    private const string AssemblyPath = "StudioLE.Workflows.NUnit.Samples.dll";
    private const string ActivityKey = "StudioLE.Workflows.NUnit.Samples.NUnitTestSamples.NUnitTestSamples_Test_Verify";
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
