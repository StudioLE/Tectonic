using System.IO;
using System.Reflection;
using Lineweights.Workflows.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class ActivityCommandTests
{
    private readonly Assembly _assembly;

    public ActivityCommandTests()
    {
        #if DEBUG
        const string configuration = "Debug";
        #else
        const string configuration = "Release";
        #endif
        string cwd = Directory.GetCurrentDirectory();
        const string pathFromRootToSamples = $"Lineweights.Flex/samples/bin/{configuration}/netstandard2.0/Lineweights.Flex.Samples.dll";
        const string pathToRoot = "../../../../../";
        string path = Path.Combine(cwd, pathToRoot, pathFromRootToSamples);
        _assembly = Assembly.LoadFile(path);
    }

    [TestCase("Flex2dSample.Execute")]
    public void ActivityCommand_CreateByMethod(string activityKey)
    {
        // Arrange
        MethodInfo? method = ActivityHelpers.GetActivityMethodByKey(_assembly, activityKey);
        if (method is null)
            throw new("Failed to get method.");
        object[] parameters = ActivityHelpers.CreateParameterInstances(method);

        // Act
        ActivityCommand activity = ActivityCommand.CreateByMethod(method, parameters);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs.Count, Is.EqualTo(5), "Inputs count");
    }

    [TestCase("Flex2dSample.Execute")]
    public void ActivityCommand_Execute(string activityKey)
    {
        // Arrange
        MethodInfo? method = ActivityHelpers.GetActivityMethodByKey(_assembly, activityKey);
        if (method is null)
            throw new("Failed to get method.");
        object[] parameters = ActivityHelpers.CreateParameterInstances(method);
        ActivityCommand activity = ActivityCommand.CreateByMethod(method, parameters);

        // Act
        dynamic outputs = activity.Execute();

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
    }
}
