namespace Tectonic;

public interface IActivityMetadata
{
    /// <summary>
    /// The unique key to identify the activity.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The name of the activity.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The description of the activity.
    /// </summary>
    public string Description { get; }
}
