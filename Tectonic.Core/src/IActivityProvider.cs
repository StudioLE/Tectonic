namespace Tectonic;

/// <summary>
/// Retrieve an <see cref="IActivity"/> by key.
/// </summary>
public interface IActivityProvider
{
    /// <summary>
    /// Retrieve an <see cref="IActivity"/> by key.
    /// </summary>
    /// <param name="activityKey">The activity key.</param>
    /// <returns>
    /// The activity.
    /// </returns>
    public IActivity? Get(string activityKey);

    /// <summary>
    /// Get the keys of all retievable activities.
    /// </summary>
    /// <returns>
    /// The activity keys.
    /// </returns>
    public IEnumerable<string> GetKeys();
}
