using System.IO;
using System.Reflection;
using Ardalis.Result;
using Lineweights.Workflows.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class ActivityRunnerTests
{
    private static readonly Assembly _assembly = GetAssembly();

    [Test]
    public void ActivityRunner_ExtractActivities()
    {
        // Arrange
        ActivityRunner runner = new();

        // Act
        Result<IReadOnlyCollection<string>> result = runner.ExtractActivities(_assembly);

        // Assert
        IReadOnlyCollection<string> activities = Validate.OrThrow(result, string.Empty);
        Assert.That(activities.Count, Is.EqualTo(5), "Activity count");
    }

    [Test]
    public void ActivityRunner_Execute()
    {
        // Arrange
        ActivityRunner runner = new();
        Result<IReadOnlyCollection<string>> activities = runner.ExtractActivities(_assembly);
        string activityId = activities.Value.FirstOrDefault() ?? throw new("Failed to extract activities.");

        // Act
        Result<object> result = runner.Execute(activityId);

        // Assert
        dynamic outputs = Validate.OrThrow(result, string.Empty);
        Assert.That(outputs, Is.Not.Null, "Not null");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(24), "Output model count");
    }

    private static Assembly GetAssembly()
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
        return Assembly.LoadFile(path);
    }
}
