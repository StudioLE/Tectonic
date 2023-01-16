using System.Reflection;
using System.Xml;
using Geometrician.Core.Execution;
using Lineweights.Diagnostics.Verification;
using NUnit.Engine;

namespace Lineweights.Diagnostics.NUnit.Execution;

/// <summary>
/// An <see cref="IActivity"/> based on a static <see cref="MethodInfo"/> in an <see cref="Assembly"/>.
/// </summary>
public sealed class NUnitActivity : IActivity
{
    private readonly object? _instance;
    private readonly ITestRunner _runner;
    private readonly TestFilter _filter;

    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Description { get; set; }

    /// <inheritdoc/>
    public object[] Inputs { get; } = Array.Empty<object>();

    public NUnitActivity(object? instance, ITestRunner runner, TestFilter filter, string key)
    {
        _instance = instance;
        _runner = runner;
        _filter = filter;
        Key = key;
        Name = Key;
        Description = Key;
    }

    /// <inheritdoc/>
    public Task<object> Execute()
    {
        // Before invocation
        bool wasVerifyEnabled = Verify.IsEnabled;
        Verify.IsEnabled = false;
        NUnitActivityResolver.IsExecuting = true;

        XmlNode? testResult = _runner.Run(null, _filter);

        // After invocation
        NUnitActivityResolver.IsExecuting = false;
        Verify.IsEnabled = wasVerifyEnabled;

        object output = NUnitActivityResolver.TestOutput ?? true;
        NUnitActivityResolver.TestOutput = null;
        return output is Task<object> task
            ? task
            : Task.FromResult(output);
    }

    /// <inheritdoc/>
    public object Clone()
    {
        return new NUnitActivity(_instance, _runner, _filter, Key)
        {
            Name = Name,
            Description = Description
        };
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}
