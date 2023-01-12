using System.Reflection;
using Geometrician.Core.Execution;
using Lineweights.Diagnostics.NUnit.Execution;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Tests.Execution;

internal sealed class NUnitActivityFactoryTests
{
    private const string AssemblyPath = "Lineweights.Core.Tests.dll";
    private const string ActivityKey = "Lineweights.Core.Tests.Geometry.CreateRuledSurfaceTests.CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);

    [Test]
    public async Task NUnitActivityFactory_AllActivityKeysInAssembly()
    {
        // Arrange
        NUnitActivityFactory factory = new();

        // Act
        string[] activities = factory.AllActivityKeysInAssembly(_assembly).ToArray();

        // Assert
        await Verify.String(activities.Join());
        Assert.That(activities.Count, Is.EqualTo(26), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void NUnitActivityFactory_TryCreateByKey(string activityKey)
    {
        // Arrange
        NUnitActivityFactory factory = new();

        // Act
        IResult<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activity.Inputs.Length, Is.EqualTo(0), "Inputs count");
    }

    [TestCase(ActivityKey)]
    public void ActivityCommand_Execute(string activityKey)
    {
        // Arrange
        NUnitActivityFactory factory = new();

        // Act
        IResult<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Act
        object outputs = activity.Execute();

        // Assert
        // TODO: obtain outputs from test fixture.
        Assert.That(outputs, Is.Not.Null, "Outputs");
        IResult<Model?> model = outputs.TryGetPropertyValue<Model?>("Model");
        Assert.That(model is Success<Model?>, "Outputs has model");
        if(model is Success<Model?> success)
            Assert.That(success?.Value?.Elements.Count, Is.EqualTo(25), "Outputs model count");
    }


}
