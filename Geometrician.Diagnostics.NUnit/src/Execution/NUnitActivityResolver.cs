using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Geometrician.Workflows.Execution;
using NUnit.Engine;
using StudioLE.Core.Results;

namespace Geometrician.Diagnostics.NUnit.Execution;

/// <inheritdoc cref="IActivityResolver"/>
public class NUnitActivityResolver : IActivityResolver, IDisposable
{
    private readonly ITestEngine _engine = TestEngineActivator.CreateInstance();

    internal static bool IsExecuting { get; set; } = false;

    internal static object? TestOutput { get; set; }

    /// <inheritdoc/>
    public IEnumerable<string> AllActivityKeysInAssembly(Assembly assembly)
    {
        using ITestRunner runner = GetTestRunner(assembly);
        return AllActivityKeysInAssembly(runner, TestFilter.Empty);
    }

    /// <inheritdoc/>
    public IResult<IActivity> Resolve(Assembly assembly, string activityKey)
    {
        TestFilter filter = new($"<filter><test>{activityKey}</test></filter>");
        ITestRunner runner = GetTestRunner(assembly);
        string? result = AllActivityKeysInAssembly(runner, filter).FirstOrDefault();
        if (result is null)
            return new Failure<IActivity>("No activity in the assembly matched the key.");
        NUnitActivity activity = new(null, runner, filter, activityKey);
        return new Success<IActivity>(activity);
    }

    private static IEnumerable<string> AllActivityKeysInAssembly(ITestRunner runner, TestFilter filter)
    {
        XmlNode testSuiteNode = runner.Explore(filter);
        XDocument testSuite = XDocument.Parse(testSuiteNode.OuterXml);
        return testSuite
            .Descendants("test-case")
            .Select(x => x.Attribute("fullname")?.Value)
            .OfType<string>();
    }

    private ITestRunner GetTestRunner(Assembly assembly)
    {
        // Create a simple test package - one assembly, no special settings
        TestPackage package = new(assembly.Location);
        // Get a runner for the test package
        return _engine.GetRunner(package);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _engine.Dispose();
    }
}
