using System.Reflection;
using Geometrician.Core.Execution;
using Lineweights.Diagnostics.NUnit.Execution;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Tests.Execution;

internal sealed class NUnitActivityResolverTests
{
    private const string AssemblyPath = "Lineweights.Core.Tests.dll";
    private const string ActivityKey = "Lineweights.Core.Tests.Geometry.CreateRuledSurfaceTests.CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    public async Task NUnitActivityResolver_AllActivityKeysInAssembly()
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        string[] activities = resolver.AllActivityKeysInAssembly(_assembly).ToArray();

        // Assert
        await Verify.String(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(26), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void NUnitActivityResolver_TryCreateByKey(string activityKey)
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activity.Inputs.Length, Is.EqualTo(0), "Inputs count");
    }
}
