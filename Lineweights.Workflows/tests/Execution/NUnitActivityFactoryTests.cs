using System.IO;
using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.NUnit.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class NUnitActivityFactoryTests
{
    private const string AssemblyPath = "Lineweights.Core.Tests.dll";
    private const string ActivityKey = "Lineweights.Core.Tests.Geometry.CreateRuledSurfaceTests.CreateRuledSurface_AsLinesByCurves_QuarterHyperbolicParaboloid";
    private readonly Assembly _assembly = Assembly.LoadFile(Path.GetFullPath(AssemblyPath));

    [Test]
    public void NUnitActivityFactory_AllActivityKeysInAssembly()
    {
        // Arrange
        NUnitActivityFactory factory = new();

        // Act
        string[] activities = factory.AllActivityKeysInAssembly(_assembly).ToArray();

        // Assert
        Assert.That(activities.Count, Is.EqualTo(22), "Activity count");
    }

    [TestCase(ActivityKey)]
    public void NUnitActivityFactory_TryCreateByKey(string activityKey)
    {
        // Arrange
        NUnitActivityFactory factory = new();

        // Act
        Result<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
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
        // Disable verify for duration of test
        bool wasVerifyEnabled = Verify.IsEnabled;
        Verify.IsEnabled = false;

        // Arrange
        NUnitActivityFactory factory = new();

        // Act
        Result<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Act
        dynamic outputs = activity.Execute();

        // Assert
        // TODO: obtain outputs from test fixture.
        Assert.That(outputs, Is.Not.Null, "Outputs");
        // Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");

        // Re-enable verify
        Verify.IsEnabled = wasVerifyEnabled;
    }


}
