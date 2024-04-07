using System.Reflection;
using NUnit.Framework;
using StudioLE.Extensions.System.Reflection;
using StudioLE.Results;

namespace Tectonic.Extensions.NUnit.Tests;

internal sealed class NUnitActivityTests
{
    private const string AssemblyPath = "Tectonic.Extensions.NUnit.Samples.dll";
    private const string ActivityKey = "Tectonic.Extensions.NUnit.Samples.NUnitTestSamples.NUnitTestSamples_Test_Verify";
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
