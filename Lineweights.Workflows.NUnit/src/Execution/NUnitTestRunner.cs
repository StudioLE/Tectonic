using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Ardalis.Result;
using Lineweights.Workflows.Execution;
using NUnit.Engine;

namespace Lineweights.Workflows.NUnit.Execution;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class NUnitTestRunner : IActivityRunner
{
    private readonly ITestEngine _engine = TestEngineActivator.CreateInstance();
    private ITestRunner? _runner;

    /// <inheritdoc/>
    public Result<IReadOnlyCollection<string>> ExtractActivities(Assembly assembly)
    {
        // Create a simple test package - one assembly, no special settings
        TestPackage package = new(assembly.Location);

        // Get a runner for the test package
        _runner = _engine.GetRunner(package);

        // Extract the activities
        XmlNode testSuiteNode = _runner.Explore(TestFilter.Empty);
        XDocument testSuite = XDocument.Parse(testSuiteNode.OuterXml);
        return testSuite
            .Descendants("test-case")
            .Select(x => x.Attribute("fullname")?.Value)
            .OfType<string>()
            .ToArray();
    }

    /// <inheritdoc/>
    public Result<object> Execute(string activityId)
    {
        if (_runner is null)
            return Result<object>.Error($"{nameof(_runner)} was null");
        TestFilter filter = new($"<filter><test>{activityId}</test></filter>");
        try
        {
            XmlNode testResult = _runner.Run(null, filter);
            return testResult;
        }
        catch (Exception e)
        {
            return Result<object>.Error($"Failed to execute runner. A {e.GetType()} exception was thrown: {e.Message}");
        }

    }
}
