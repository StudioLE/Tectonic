using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Ardalis.Result;
using Lineweights.Workflows.Execution;
using NUnit.Engine;

namespace Lineweights.Workflows.NUnit.Execution;

/// <inheritdoc cref="IActivityFactory"/>
public class NUnitActivityFactory : IActivityFactory, IDisposable
{
    private readonly ITestEngine _engine = TestEngineActivator.CreateInstance();

    /// <inheritdoc />
    public IEnumerable<string> AllActivityKeysInAssembly(Assembly assembly)
    {
        using ITestRunner runner = GetTestRunner(assembly);
        return AllActivityKeysInAssembly(runner, TestFilter.Empty);
    }

    /// <inheritdoc />
    public Result<ActivityCommand> TryCreateByKey(Assembly assembly, string activityKey)
    {
        object[] inputs = Array.Empty<object>();
        TestFilter filter = new($"<filter><test>{activityKey}</test></filter>");
        ITestRunner runner = GetTestRunner(assembly);
        string? result = AllActivityKeysInAssembly(runner, filter).FirstOrDefault();
        if(result is null)
            return Result<ActivityCommand>.Error("No activity in the assembly matched the key.");
        Func<object[], object> invocation = CreateInvocation(runner, filter);
        Action dispose = () => runner.Dispose();
        return new ActivityCommand
        {
            Name = activityKey,
            Key = activityKey,
            Inputs = inputs,
            Invocation = invocation,
            OnDispose = dispose
        };
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

    private static Func<object[], object> CreateInvocation(ITestRunner runner, TestFilter filter)
    {
        return inputs => runner.Run(null, filter);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _engine.Dispose();
    }
}
