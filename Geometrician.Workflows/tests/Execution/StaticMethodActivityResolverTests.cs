using System.Reflection;
using Geometrician.Workflows.Execution;
using NUnit.Framework;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Workflows.Tests.Execution;

internal sealed class StaticMethodActivityResolverTests
{
    private const string AssemblyPath = "Geometrician.Flex.Samples.dll";
    private const string ActivityKey = "Flex2dSample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    public void StaticMethodActivityResolver_AllActivityMethodsInAssembly()
    {
        // Arrange
        // Act
        MethodInfo[] activities = StaticMethodActivityResolver.AllActivityMethodsInAssembly(_assembly).ToArray();

        // Assert
        Assert.That(activities.Count, Is.EqualTo(6), "Activity count");
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
        IActivity activity = Validate.OrThrow(result);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activity.Inputs.Length, Is.EqualTo(5), "Inputs count");
    }
}
