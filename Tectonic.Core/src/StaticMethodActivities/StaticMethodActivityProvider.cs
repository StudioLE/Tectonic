using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Tectonic.StaticMethodActivities;

public class StaticMethodActivityProvider : IActivityProvider
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IReadOnlyDictionary<string, MethodInfo> _activityMethods;


    public StaticMethodActivityProvider(
        ILoggerFactory loggerFactory,
        IReadOnlyDictionary<string, MethodInfo> activityMethods)
    {
        _loggerFactory = loggerFactory;
        _activityMethods = activityMethods;
    }

    /// <inheritdoc/>
    public IActivity? Get(string activityKey)
    {
        if (!_activityMethods.TryGetValue(activityKey, out MethodInfo? method))
            return null;
        ILogger<StaticMethodActivity> logger = _loggerFactory.CreateLogger<StaticMethodActivity>();
        StaticMethodActivity activity = new(logger, method, activityKey);
        return activity;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetKeys()
    {
        return _activityMethods.Keys;
    }
}
