namespace Geometrician.Core.Visualization;

public class VisualizationConfiguration
{
    public HttpClient HttpClient { get; } = new()
    {
        Timeout = TimeSpan.FromMilliseconds(200),
        BaseAddress = new(BaseAddress)
    };

    /// <summary>
    /// The path of the asset API endpoint.
    /// </summary>
    public const string BaseAddress = "http://localhost:3000";

    /// <summary>
    /// The path of the visualize API endpoint.
    /// </summary>
    public const string VisualizeRoute = "visualize";

    internal Dictionary<string, Type> ContentTypes { get; } = new();

    public VisualizationConfiguration RegisterContentType(string contentType, Type component)
    {
        ContentTypes[contentType] = component;

        return this;
    }
}
