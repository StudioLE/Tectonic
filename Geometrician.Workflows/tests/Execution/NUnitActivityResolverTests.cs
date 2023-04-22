using System.Reflection;
using Geometrician.Diagnostics.NUnit.Execution;
using Geometrician.Workflows.Execution;
using NUnit.Framework;
using StudioLE.Core.Results;
using StudioLE.Core.System;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Workflows.Tests.Execution;

internal sealed class NUnitActivityResolverTests
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private const string AssemblyPath = "Geometrician.Core.Tests.dll";
    private const string ActivityKey = "Geometrician.Core.Tests.Geometry.CreateRuledSurfaceTests.CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    [Explicit("Requires Geometrician.Core.Tests.dll")]
    public async Task NUnitActivityResolver_AllActivityKeysInAssembly()
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        string[] activities = resolver.AllActivityKeysInAssembly(_assembly).ToArray();

        // Assert
        await _verify.String(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(26), "Activity count");
    }

    [TestCase(ActivityKey)]
    [Explicit("Requires Geometrician.Core.Tests.dll")]
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
