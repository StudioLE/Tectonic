using System.IO;
using System.Reflection;
using Lineweights.Workflows.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class ActivityRunnerTests
{
    private static readonly Assembly _assembly = GetAssembly();

    [Test]
    public void ActivityRunner_All()
    {
        // Arrange
        ActivityBuilder builder = new();

        // Act
        builder.SetAssembly(_assembly);

        // Assert
        if (builder.State is not ActivityBuilder.AssemblySetState assemblySetState)
            throw new("Incorrect state.");
        Assert.That(assemblySetState.Activities.Count, Is.EqualTo(5), "Activity count");

        // Arrange
        string activityName = assemblySetState.ActivityNames.First();

        // Act
        var result = builder.SetActivity(activityName);

        // Assert
        Assert.That(result.Errors.Count, Is.EqualTo(0), "Error count");
        if (builder.State is not ActivityBuilder.ActivitySetState activitySetState)
            throw new("Incorrect state.");
        Assert.That(activitySetState.Inputs, Is.Not.Null, "Not null");
        Assert.That(activitySetState.Inputs.Count, Is.EqualTo(1), "Inputs count");

        // Arrange
        // Act
        builder.Build();

        // Assert
        if (builder.State is not ActivityBuilder.BuiltState builtState)
            throw new("Incorrect state.");
        Assert.That(builtState.Command, Is.Not.Null, "Command is not null");
        Assert.That(builtState.Command.Name, Is.Not.Empty, "Command name");
        Assert.That(builtState.Command.Inputs, Is.Not.Empty, "Command inputs");

        // Arrange
        ActivityCommand activity = builtState.Command;

        // Act
        dynamic outputs = activity.Execute();

        // Assert
        if (builder.State is not ActivityBuilder.BuiltState)
            throw new("Incorrect state.");
        Assert.That(outputs, Is.Not.Null, "Not null");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(24), "Output model count");

        // Arrange
        // Act
        activity.Execute();
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
