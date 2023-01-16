using System.Reflection;
using Geometrician.Core.Execution;
using Lineweights.Diagnostics.NUnit.Execution;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Tests.Execution;

internal sealed class NUnitActivityTests
{
    private const string AssemblyPath = "Lineweights.Core.Tests.dll";
    private const string ActivityKey = "Lineweights.Core.Tests.Geometry.CreateRuledSurfaceTests.CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [TestCase(ActivityKey)]
    public async Task NUnitActivity_Execute(string activityKey)
    {
        // Arrange
        NUnitActivityResolver resolver = new();

        // Act
        IResult<IActivity> result = resolver.Resolve(_assembly, activityKey);
        IActivity activity = Validate.OrThrow(result);

        // Act
        object outputs = await activity.Execute();

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        IResult<Model?> model = outputs.TryGetPropertyValue<Model?>("Model");
        Assert.That(model is Success<Model?>, "Outputs has model");
        if (model is Success<Model?> success)
            Assert.That(success?.Value?.Elements.Count, Is.EqualTo(25), "Outputs model count");
    }
}
