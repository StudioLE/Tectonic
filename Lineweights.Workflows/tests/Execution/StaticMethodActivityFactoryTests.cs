using System.IO;
using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class StaticMethodActivityFactoryTests
{
    private readonly Assembly _assembly;

    public StaticMethodActivityFactoryTests()
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

    [Test]
    public void StaticMethodActivityFactory_AllActivityMethodsInAssembly()
    {
        // Arrange
        // Act
        MethodInfo[] activities = StaticMethodActivityFactory.AllActivityMethodsInAssembly(_assembly).ToArray();

        // Assert
        Assert.That(activities.Count, Is.EqualTo(6), "Activity count");
    }

    [TestCase("Flex2dSample.Execute")]
    public void StaticMethodActivityFactory_GetActivityMethodByName(string activityKey)
    {
        // Arrange
        // Act
        MethodInfo? activity = StaticMethodActivityFactory.GetActivityMethodByKey(_assembly, activityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
    }

    [TestCase("Flex2dSample.Execute")]
    public void StaticMethodActivityFactory_CreateParameterInstances(string activityKey)
    {
        // Arrange
        MethodInfo? method = StaticMethodActivityFactory.GetActivityMethodByKey(_assembly, activityKey);
        if (method is null)
            throw new("Failed to get method.");

        // Act
        object[] parameters = StaticMethodActivityFactory.CreateParameterInstances(method);

        // Assert
        Assert.That(parameters, Is.Not.Null);
        Assert.That(parameters.Count, Is.EqualTo(5), "Parameters count");
    }

    [TestCase("Flex2dSample.Execute")]
    public void StaticMethodActivityFactory_TryCreateByKey(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();

        // Act
        Result<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Assert
        Assert.That(activity, Is.Not.Null, "Command");
        Assert.That(activity.Name, Is.Not.Empty, "Name");
        Assert.That(activity.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activity.Inputs.Length, Is.EqualTo(5), "Inputs count");
    }

    [TestCase("Flex2dSample.Execute")]
    public void ActivityCommand_Execute(string activityKey)
    {
        // Arrange
        StaticMethodActivityFactory factory = new();

        // Act
        Result<ActivityCommand> result = factory.TryCreateByKey(_assembly, activityKey);
        ActivityCommand activity = Validate.OrThrow(result);

        // Act
        dynamic outputs = activity.Execute();

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
    }
}
