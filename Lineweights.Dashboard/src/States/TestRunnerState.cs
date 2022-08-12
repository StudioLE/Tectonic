using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using NUnit.Engine;

namespace Lineweights.Dashboard.States;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class TestRunnerState
{
    private readonly ILogger<TestRunnerState> _logger;

    // TODO: Rename to NUnitTestRunner and implement as interface.

    /// <summary>
    /// The engine used by the test runner.
    /// </summary>
    public ITestEngine Engine { get; } = TestEngineActivator.CreateInstance();

    /// <summary>
    /// The test runner.
    /// </summary>
    public ITestRunner? Runner { get; set; }

    /// <summary>
    /// The tests loaded from the _testRunner.
    /// </summary>
    public IReadOnlyCollection<string> Tests { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The currently selected test assembly.
    /// </summary>
    public string SelectedPackage { get; set; } = @"E:\Repos\Hypar\Lineweights\Lineweights.Workflows\tests\bin\Debug\net6.0\Lineweights.Workflows.Tests.dll";

    /// <summary>
    /// The currently selected test.
    /// </summary>
    public string SelectedTest { get; set; } = string.Empty;

    public TestRunnerState(ILogger<TestRunnerState> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Load the test assembly.
    /// </summary>
    public void Load()
    {
        _logger.LogDebug($"{nameof(Load)} called on {SelectedPackage}.");
        FileInfo dll = new(SelectedPackage);

        if (!dll.Exists)
        {
            _logger.LogError($"Failed to load test assembly. File not found.");
            return;
        }

        Assembly assembly = Assembly.LoadFrom(dll.FullName);

        // Create a simple test package - one assembly, no special settings
        TestPackage package = new(dll.FullName);

        // Get a runner for the test package
        Runner = Engine.GetRunner(package);

        XmlNode testSuiteNode = Runner.Explore(TestFilter.Empty);
        XDocument testSuite = XDocument.Parse(testSuiteNode.OuterXml);
        Tests = testSuite
            .Descendants("test-case")
            .Select(x => x.Attribute("fullname")?.Value)
            .OfType<string>()
            .ToArray();
        SelectedTest = Tests.First();
    }

    /// <summary>
    /// Run the selected test.
    /// </summary>
    public void Execute()
    {
        _logger.LogDebug($"{nameof(Execute)}() called on {SelectedPackage} {SelectedTest}.");
        if (Runner is null)
        {
            _logger.LogError($"{nameof(Runner)} was null");
            return;
        }

        TestFilter filter = new($"<filter><test>{SelectedTest}</test></filter>");

        // Run all the tests in the assembly
        _logger.LogDebug("Runner started.");
        try
        {
            XmlNode testResult = Runner.Run(null, filter);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to execute runner. A {e.GetType()} exception was thrown: {e.Message}");
        }

        _logger.LogDebug("Runner finished.");
    }
}
