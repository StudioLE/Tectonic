namespace Lineweights.Workflows.Visualization;

public class GeometricianService
{
    public HttpClient HttpClient { get; } = new()
    {
        Timeout = TimeSpan.FromMilliseconds(50)
    };

    /// <summary>
    /// The path of the asset API endpoint.
    /// </summary>
    public const string AssetUrl = $"http://localhost:3000/{AssetRoute}";

    // TODO: This should be determined by settings
    /// <summary>
    /// The path of the asset API endpoint.
    /// </summary>
    public const string AssetRoute = "asset";
}
