using System.IO;
using System.Reflection;
using Lineweights.Workflows.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class ActivityHelpersTests
{
    private readonly Assembly _assembly;

    public ActivityHelpersTests()
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
    public void ActivityHelpers_GetActivityMethods()
    {
        // Arrange
        // Act
        MethodInfo[] activities = ActivityHelpers.GetActivityMethods(_assembly).ToArray();

        // Assert
        Assert.That(activities.Count, Is.EqualTo(6), "Activity count");
    }

    [TestCase("Flex2dSample.Execute")]
    public void ActivityHelpers_GetActivityMethodByName(string activityKey)
    {
        // Arrange
        MethodInfo[] activities = ActivityHelpers.GetActivityMethods(_assembly).ToArray();
        string[] activityNames = activities.Select(ActivityHelpers.GetActivityKey).ToArray();

        // Act
        MethodInfo? activity = ActivityHelpers.GetActivityMethodByKey(_assembly, activityKey);

        // Assert
        Assert.That(activity, Is.Not.Null);
    }

    [TestCase("Flex2dSample.Execute")]
    public void ActivityHelpers_CreateParameterInstances(string activityKey)
    {
        // Arrange
        MethodInfo? method = ActivityHelpers.GetActivityMethodByKey(_assembly, activityKey);
        if (method is null)
            throw new("Failed to get method.");

        // Act
        object[] parameters = ActivityHelpers.CreateParameterInstances(method);

        // Assert
        Assert.That(parameters, Is.Not.Null);
        Assert.That(parameters.Count, Is.EqualTo(5), "Parameters count");
    }
}
