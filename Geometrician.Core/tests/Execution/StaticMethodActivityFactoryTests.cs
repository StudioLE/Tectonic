using System.Reflection;
using Geometrician.Core.Execution;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Tests.Execution;

internal sealed class StaticMethodActivityFactoryTests
{
    private const string AssemblyPath = "Lineweights.Flex.Samples.dll";
    private const string ActivityKey = "Flex2dSample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    public void StaticMethodActivityFactory_AllActivityMethodsInAssembly()
    {
        // Arrange
        // Act
        MethodInfo[] activities = StaticMethodActivityFactory.AllActivityMethodsInAssembly(_assembly).ToArray();

        // Assert
        Assert.That(activities.Count, Is.EqualTo(6), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void StaticMethodActivityFactory_GetActivityMethodByName(string activityKey)
    {
        // Arrange
        // Act
        MethodInfo? activity = StaticMethodActivityFactory.GetActivityMethodByKey(_assembly, activityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
    }

    [TestCase(ActivityKey)]
    public void StaticMethodActivityFactory_TryCreateByKey(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();

        // Act
        IResult<IActivity> result = factory.TryCreateByKey(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activity.Inputs.Length, Is.EqualTo(5), "Inputs count");
    }
}
