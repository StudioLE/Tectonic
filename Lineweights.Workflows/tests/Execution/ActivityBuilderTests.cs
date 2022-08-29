using System.IO;
using System.Reflection;
using Lineweights.Workflows.Execution;

namespace Lineweights.Workflows.Tests.Execution;

internal sealed class ActivityBuilderTests
{
    private static readonly Assembly _assembly = GetAssembly();

    [Test]
    public void ActivityBuilder_Build()
    {
        // Arrange
        ActivityBuilder builder = new();

        // Act
        builder.SetAssembly(_assembly);

        // Assert
        if (builder.State is not ActivityBuilder.AssemblySetState assemblySetState)
            throw new("Incorrect state.");
        Assert.That(assemblySetState.Activities.Count, Is.EqualTo(6), "Activity count");

        // Arrange
        const string activityName = "Flex2dSample.Execute";

        // Act
        var result = builder.SetActivity(activityName);

        // Assert
        Assert.That(result.Errors.Count, Is.EqualTo(0), "Error count");
        if (builder.State is not ActivityBuilder.ActivitySetState activitySetState)
            throw new("Incorrect state.");
        Assert.That(activitySetState.Inputs, Is.Not.Null, "Inputs");
        Assert.That(activitySetState.Inputs.Count, Is.EqualTo(5), "Inputs count");

        // Arrange
        // Act
        builder.Build();

        // Assert
        if (builder.State is not ActivityBuilder.BuiltState builtState)
            throw new("Incorrect state.");
        Assert.That(builtState.Command, Is.Not.Null, "Command");
        Assert.That(builtState.Command.Name, Is.Not.Empty, "Command name");
        Assert.That(builtState.Command.Inputs, Is.Not.Empty, "Command inputs");

        // Arrange
        ActivityCommand activity = builtState.Command;

        // Act
        dynamic outputs = activity.Execute();

        // Assert
        if (builder.State is not ActivityBuilder.BuiltState)
            throw new("Incorrect state.");
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");

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
