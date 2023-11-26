using System.Reflection;
using NUnit.Framework;
using StudioLE.Results;
using StudioLE.Extensions.System.Reflection;

namespace Cascade.Workflows.NUnit.Tests;

internal sealed class NUnitActivityTests
{
    private const string AssemblyPath = "Cascade.Workflows.NUnit.Samples.dll";
    private const string ActivityKey = "Cascade.Workflows.NUnit.Samples.NUnitTestSamples.NUnitTestSamples_Test_Verify";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [TestCase(ActivityKey)]
    public async Task NUnitActivity_Execute(string activityKey)
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = result.GetValueOrThrow();

        // Act
        object outputs = await activity.Execute(null!);

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
    }
}
