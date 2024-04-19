using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using NUnit.Engine;
using StudioLE.Patterns;

namespace Tectonic.Extensions.NUnit;

/// <summary>
/// Build an <see cref="NUnitActivityProvider"/>.
/// </summary>
public class NUnitActivityProviderBuilder : IBuilder<NUnitActivityProvider>, IDisposable
{
    private readonly ITestEngine _engine = TestEngineActivator.CreateInstance();
    private readonly Dictionary<string, Assembly> _activities = new();

    /// <summary>
    /// Add all <see cref="IActivity"/> from <paramref name="assembly"/>.
    /// </summary>
    /// <paramref name="assembly">The assembly.</paramref>
    /// <returns>
    /// The <see cref="NUnitActivityProvider"/> for fluent chaining.
    /// </returns>
    public NUnitActivityProviderBuilder Add(Assembly assembly, TestFilter? filter = null)
    {
        filter ??= TestFilter.Empty;
        IEnumerable<string> keys = GetTests(assembly, filter);
        foreach (string key in keys)
            _activities[key] = assembly;
        return this;
    }

    /// <inheritdoc/>
    public NUnitActivityProvider Build()
    {
        return new(_activities);
    }

    /// <summary>
    /// Get all activity methods in the assembly.
    /// </summary>
    private IEnumerable<string> GetTests(Assembly assembly, TestFilter filter)
    {
        TestPackage package = new(assembly.Location);
        ITestRunner runner = _engine.GetRunner(package);
        XmlNode testSuiteNode = runner.Explore(filter);
        XDocument testSuite = XDocument.Parse(testSuiteNode.OuterXml);
        return testSuite
            .Descendants("test-case")
            .Select(x => x.Attribute("fullname")?.Value)
            .OfType<string>();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _engine.Dispose();
    }
}
