using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tectonic;

/// <inheritdoc cref="IActivityProvider"/>
public class ActivityProvider : IActivityProvider
{
    private readonly ILogger<ActivityProvider> _logger;
    private readonly IServiceProvider _services;
    private readonly IReadOnlyDictionary<string, Type> _activityTypes;

    /// <summary>
    /// DI constructor for <see cref="ActivityProvider"/>.
    /// </summary>
    public ActivityProvider(IServiceProvider services, IReadOnlyDictionary<string, Type> activityTypes)
    {
        _logger = services.GetRequiredService<ILogger<ActivityProvider>>();
        _services = services;
        _activityTypes = activityTypes;
    }

    /// <inheritdoc />
    public IActivity? Get(string activityKey)
    {
        if (!_activityTypes.TryGetValue(activityKey, out Type? activityType))
            return null;
        IActivity? activity = _services.GetService(activityType) as IActivity;
        if (activity is null)
            _logger.LogError($"Failed to create activity by dependency injection: {activityKey}.");
        return activity;
    }

    /// <inheritdoc />
    public IEnumerable<string> GetKeys()
    {
        return _activityTypes.Keys;
    }
}
