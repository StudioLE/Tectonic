using System.Reflection;
using NUnit.Engine;

namespace Tectonic.Extensions.NUnit;

public class NUnitActivityProvider : IActivityProvider, IDisposable
{
    private readonly ITestEngine _engine = TestEngineActivator.CreateInstance();
    private readonly IReadOnlyDictionary<string, Assembly> _activities;

    public NUnitActivityProvider(IReadOnlyDictionary<string, Assembly> activities)
    {
        _activities = activities;
    }

    /// <inheritdoc/>
    public IActivity? Get(string activityKey)
    {
        if (!_activities.TryGetValue(activityKey, out Assembly? assembly))
            return null;
        TestFilter filter = new($"<filter><test>{activityKey}</test></filter>");
        TestPackage package = new(assembly.Location);
        ITestRunner runner = _engine.GetRunner(package);
        NUnitActivity activity = new(runner, filter, activityKey);
        return activity;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetKeys()
    {
        return _activities.Keys;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _engine.Dispose();
    }
}
