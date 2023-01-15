using System.Reflection;
using Geometrician.Core.Execution;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Tests.Execution;

internal sealed class MethodActivityTests
{
    private const string AssemblyPath = "Lineweights.Flex.Samples.dll";
    private const string ActivityKey = "Flex2dSample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [TestCase(ActivityKey)]
    public void MethodActivity_Inputs(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();
        IResult<IActivity> result = factory.TryCreateByKey(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Act
        // Assert
        Assert.That(activity.Inputs, Is.Not.Null);
        Assert.That(activity.Inputs.Count, Is.EqualTo(5), "Parameters count");
    }

    [TestCase(ActivityKey)]
    public async Task MethodActivity_Execute(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();

        // Act
        IResult<IActivity> result = factory.TryCreateByKey(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Act
        dynamic outputs = await activity.Execute();

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
    }
}
