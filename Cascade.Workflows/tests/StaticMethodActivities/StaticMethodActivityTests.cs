using System.Reflection;
using Cascade.Workflows.StaticMethodActivities;
using NUnit.Framework;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Cascade.Workflows.Tests.StaticMethodActivities;

internal sealed class StaticMethodActivityTests
{
    private const string AssemblyPath = "Cascade.Workflows.Samples.dll";
    private const string ActivityKey = "StaticMethodActivityExample.Execute";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);


    [TestCase(ActivityKey)]
    public void StaticMethodActivity_GetInputType(string activityKey)
    {
        // Arrange
        StaticMethodActivityResolver resolver = new();
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = result.GetValueOrThrow();

        // Act
        Type inputType = activity.GetInputType();

        // Assert
        Assert.That(inputType.FullName, Is.EqualTo("Cascade.Workflows.Samples.StaticMethodActivityExample+Inputs"));
    }

    [TestCase(ActivityKey)]
    public async Task StaticMethodActivity_Execute(string activityKey)
    {
        // Arrange
        StaticMethodActivityResolver resolver = new();
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = result.GetValueOrThrow();
        Type inputType = activity.GetInputType();
        object inputs = Activator.CreateInstance(inputType) ?? throw new("Failed to create inputs.");

        // Act
        dynamic outputs = await activity.Execute(inputs);

        // Assert
        Assert.That(outputs, Is.Not.Null, "Output");
        Assert.That(outputs.IsValid, Is.True, "Output property");
    }
}
