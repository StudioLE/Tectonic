using NUnit.Engine;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml;

namespace Lineweights.Dashboard;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class TestRunnerState
{
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
    public string SelectedPackage { get; set; } = @"E:\Repos\Hypar\Lineweights\Lineweights.Flex\samples\bin\Debug\net6.0\Lineweights.Flex.Samples.dll";

    /// <summary>
    /// The currently selected test.
    /// </summary>
    public string SelectedTest { get; set; } = string.Empty;

    /// <summary>
    /// Load the test assembly.
    /// </summary>
    public void Load()
    {
        FileInfo dll = new(SelectedPackage);

        if (!dll.Exists)
            throw new("Failed to load test assembly. File not found.");

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
        if (Runner is null)
        {
            Console.WriteLine("_testRunner is null");
            return;
        }

        TestFilter filter = new($"<filter><test>{SelectedTest}</test></filter>");

        // Run all the tests in the assembly
        XmlNode testResult = Runner.Run(null, filter);
    }
}
