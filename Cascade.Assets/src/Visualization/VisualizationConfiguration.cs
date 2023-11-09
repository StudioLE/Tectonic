namespace Cascade.Assets.Visualization;

public class VisualizationConfiguration
{
    // TODO: This should be set via configuration file

    /// <summary>
    /// The path of the asset API endpoint.
    /// </summary>
    public const string BaseAddress = "http://localhost:3000";

    /// <summary>
    /// The path of the visualize API endpoint.
    /// </summary>
    public const string VisualizeRoute = "visualize";

    /// <summary>
    /// The path of the storage API endpoint.
    /// </summary>
    public const string StorageRoute = "storage";

    public HttpClient HttpClient { get; } = new()
    {
        Timeout = TimeSpan.FromMilliseconds(200),
        BaseAddress = new(BaseAddress)
    };
}
