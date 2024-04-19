using StudioLE.Patterns;

namespace Tectonic;

/// <summary>
/// Build an <see cref="ActivityProvider"/>.
/// </summary>
public class ActivityProviderBuilder : IBuilder<ActivityProvider>
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Type> _activityTypes = new();

    public ActivityProviderBuilder(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Add an <see cref="IActivity"/> to the provider retrievable by <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key used to retrieve the activity.</param>
    /// <typeparam name="TActivity">The activity.</typeparam>
    /// <returns>
    /// The <see cref="ActivityProviderBuilder"/> for fluent chaining.
    /// </returns>
    public ActivityProviderBuilder Add<TActivity>(string key) where TActivity : IActivity
    {
        Type type = typeof(TActivity);
        _activityTypes[key] = type;
        return this;
    }

    /// <inheritdoc />
    public ActivityProvider Build()
    {
        return new(_services, _activityTypes);
    }
}
