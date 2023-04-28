using System.Reflection;
using Geometrician.Workflows.Execution;
using NUnit.Framework;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Workflows.Tests.Execution;

internal sealed class StaticMethodActivityTests
{
    private const string AssemblyPath = "Geometrician.Flex.Samples.dll";
    private const string ActivityKey = "Flex2dSample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [TestCase(ActivityKey)]
    public void StaticMethodActivity_Inputs(string activityKey)
    {
        // Arrange
        StaticMethodActivityResolver resolver = new();
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Act
        // Assert
        Assert.That(activity.Inputs, Is.Not.Null);
        Assert.That(activity.Inputs.Count, Is.EqualTo(5), "Parameters count");
    }

    [TestCase(ActivityKey)]
    public async Task StaticMethodActivity_Execute(string activityKey)
    {
        // Arrange
        StaticMethodActivityResolver resolver = new();

        // Act
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Act
        dynamic outputs = await activity.Execute();

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
    }
}
