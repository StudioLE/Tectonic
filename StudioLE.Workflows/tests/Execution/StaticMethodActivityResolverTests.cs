using System.Reflection;
using NUnit.Framework;
using StudioLE.Core.Results;
using StudioLE.Core.System;
using StudioLE.Verify.NUnit;
using StudioLE.Workflows.Abstractions;
using StudioLE.Workflows.StaticMethodActivities;

namespace StudioLE.Workflows.Tests.Execution;

internal sealed class StaticMethodActivityResolverTests
{
    private readonly Verify.Verify _verify = new(new NUnitVerifyContext());
    private const string AssemblyPath = "StudioLE.Workflows.Samples.dll";
    private const string ActivityKey = "StaticMethodActivityExample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    public async Task StaticMethodActivityResolver_AllActivityKeysInAssembly()
    {
        // Arrange
        StaticMethodActivityResolver resolver = new();

        // Act
        string[] activities = resolver.AllActivityKeysInAssembly(_assembly).ToArray();

        // Assert
        await _verify.String(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(1), "Activity count");
    }

    [Test]
    public void StaticMethodActivityResolver_AllActivityMethodsInAssembly()
    {
        // Arrange
        // Act
        MethodInfo[] activities = StaticMethodActivityResolver.AllActivityMethodsInAssembly(_assembly).ToArray();

        // Assert
        Assert.That(activities.Count, Is.EqualTo(1), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void StaticMethodActivityResolver_GetActivityMethodByName(string activityKey)
    {
        // Arrange
        // Act
        MethodInfo? activity = StaticMethodActivityResolver.GetActivityMethodByKey(_assembly, activityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
    }

    [TestCase(ActivityKey)]
    public void StaticMethodActivityResolver_TryCreateByKey(string activityKey)
    {
        // Arrange
        StaticMethodActivityResolver resolver = new();

        // Act
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = result.GetValueOrThrow();

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.GetName(), Is.Not.Empty, "Name");
    }
}
