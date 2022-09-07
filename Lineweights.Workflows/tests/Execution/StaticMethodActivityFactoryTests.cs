using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows.Execution;
using StudioLE.Core.System;

namespace Lineweights.Workflows.Tests.Execution;

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
    public void StaticMethodActivityFactory_CreateParameterInstances(string activityKey)
    {
        // Arrange
        MethodInfo? method = StaticMethodActivityFactory.GetActivityMethodByKey(_assembly, activityKey);
        if (method is null)
            throw new("Failed to get method.");

        // Act
        object[] parameters = StaticMethodActivityFactory.CreateParameterInstances(method);

        // Assert
        Assert.That(parameters, Is.Not.Null);
        Assert.That(parameters.Count, Is.EqualTo(5), "Parameters count");
    }

    [TestCase(ActivityKey)]
    public void StaticMethodActivityFactory_TryCreateByKey(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();

        // Act
        Result<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activity.Inputs.Length, Is.EqualTo(5), "Inputs count");
    }

    [TestCase(ActivityKey)]
    public void ActivityCommand_Execute(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();

        // Act
        Result<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Act
        dynamic outputs = activity.Execute();

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
    }
}
