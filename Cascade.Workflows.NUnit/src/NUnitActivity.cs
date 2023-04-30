using System.Reflection;
using System.Xml;
using NUnit.Engine;
using StudioLE.Verify;

namespace Cascade.Workflows.NUnit;

/// <summary>
/// An <see cref="IActivity"/> based on a static <see cref="MethodInfo"/> in an <see cref="Assembly"/>.
/// </summary>
public sealed class NUnitActivity : IActivity<object, object>, IActivityMetadata
{
    private readonly ITestRunner _runner;
    private readonly TestFilter _filter;

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
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
    public Task<object> Execute(object input)
    {
        // Before invocation
        bool wasVerifyEnabled = Verify.IsEnabled;
        Verify.IsEnabled = false;
        NUnitActivityResolver.IsExecuting = true;

        XmlNode? testResult = _runner.Run(null, _filter);

        // After invocation
        NUnitActivityResolver.IsExecuting = false;
        Verify.IsEnabled = wasVerifyEnabled;

        // TODO: NUnitActivityResolver.TestOutput has been intentionally disabled as it's too complex. Investigate an alternative.
        object output = NUnitActivityResolver.TestOutput ?? true;
        NUnitActivityResolver.TestOutput = null;
        return output is Task<object> task
            ? task
            : Task.FromResult(output);
    }
}
