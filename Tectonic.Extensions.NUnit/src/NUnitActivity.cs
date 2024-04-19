using System.Reflection;
using System.Xml;
using NUnit.Engine;

namespace Tectonic.Extensions.NUnit;

/// <summary>
/// An <see cref="IActivity"/> based on a static <see cref="MethodInfo"/> in an <see cref="Assembly"/>.
/// </summary>
public sealed class NUnitActivity : ActivityBase<object, XmlNode?>, IActivityMetadata, IDisposable
{
    private readonly ITestRunner _runner;
    private readonly TestFilter _filter;

    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Description { get; }

    public NUnitActivity(ITestRunner runner, TestFilter filter, string key)
    {
        _runner = runner;
        _filter = filter;
        Key = key;
        Name = Key;
        Description = Key;
    }

    /// <inheritdoc/>
    public override Task<XmlNode?> Execute(object input)
    {
        XmlNode? testResult = _runner.Run(null, _filter);
        return Task.FromResult<XmlNode?>(testResult);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _runner.Dispose();
    }
}
