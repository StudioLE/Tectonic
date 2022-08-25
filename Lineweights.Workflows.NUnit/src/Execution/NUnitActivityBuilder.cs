using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Ardalis.Result;
using Lineweights.Workflows.Execution;
using NUnit.Engine;

namespace Lineweights.Workflows.NUnit.Execution;

/// <summary>
/// An <see cref="ActivityBuilder"/> to create commands from NUnit tests.
/// </summary>
public class NUnitActivityBuilder : ActivityBuilder
{
    private readonly ITestEngine _engine = TestEngineActivator.CreateInstance();
    private ITestRunner? _runner;

    public override Result<object> SetAssembly(Assembly assembly)
    {
        if (State is not InitialState state)
            return Result<object>.Error($"{nameof(SetAssembly)} can only be called when {nameof(State)} is {nameof(InitialState)}.");

        string assemblyName = assembly.GetName().Name;

        // Create a simple test package - one assembly, no special settings
        TestPackage package = new(assembly.Location);

        // Get a runner for the test package
        _runner = _engine.GetRunner(package);

        // Extract the activities
        XmlNode testSuiteNode = _runner.Explore(TestFilter.Empty);
        XDocument testSuite = XDocument.Parse(testSuiteNode.OuterXml);

        string[] activityNames = testSuite
            .Descendants("test-case")
            .Select(x => x.Attribute("fullname")?.Value)
            .OfType<string>()
            .ToArray();

        // Dictionary<string, MethodInfo> activities = activityNames.ToDictionary(x => x, x => (MethodInfo)default!);

        State = new AssemblySetState
        {
            Assembly = assembly,
            AssemblyName = assemblyName,
            // Activities = activities,
            ActivityNames = activityNames
        };

        return true;
    }

    public override Result<object> SetActivity(string activityName)
    {
        if (State is not AssemblySetState state)
            return Result<object>.Error($"{nameof(SetActivity)} can only be called when {nameof(State)} is {nameof(AssemblySetState)}.");

        // object[] inputs = Array.Empty<object>();
        // MethodInfo activity = null!;

        State = new ActivitySetState
        {
            Assembly = state.Assembly,
            AssemblyName = state.AssemblyName,
            Activities = state.Activities,
            ActivityNames = state.ActivityNames,
            // Activity = activity,
            ActivityName = activityName,
            // Inputs = inputs
        };

        return true;
    }

    public override Result<object> Build()
    {
        if (State is not ActivitySetState state)
            return Result<object>.Error($"{nameof(Build)} can only be called when {nameof(State)} is {nameof(ActivitySetState)}.");
        if (_runner is null)
            return Result<object>.Error($"{nameof(_runner)} was null");

        TestFilter filter = new($"<filter><test>{state.ActivityName}</test></filter>");

        object[] inputs = state.Inputs.ToArray();
        Func<object[], object> function = x => _runner.Run(null, filter);
        ActivityCommand command = new(state.ActivityName, inputs, function);

        State = new BuiltState
        {
            Assembly = state.Assembly,
            AssemblyName = state.AssemblyName,
            Activities = state.Activities,
            ActivityNames = state.ActivityNames,
            ActivityName = state.ActivityName,
            Inputs = state.Inputs,
            Command = command,
        };

        return true;
    }
}
