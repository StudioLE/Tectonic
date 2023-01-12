using Lineweights.Core.Documents;

namespace Lineweights.Workflows.Visualization;

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

    internal Dictionary<Type, Type[]> AssetFactories { get; } = new();

    internal Dictionary<string, Type> ContentTypes { get; } = new();

    internal Dictionary<Type, int> AssertFactoriesOrder { get; } = new();

    public VisualizationConfiguration RegisterAssetFactory<TSource, TAssetFactory>(int? order = null) where TAssetFactory : IAssetFactory<TSource, IAsset>
    {
        order ??= AssertFactoriesOrder.Count + 100;
        Type sourceType = typeof(TSource);
        Type converterType = typeof(TAssetFactory);
        Type[] converters = AssetFactories.ContainsKey(sourceType)
            ? AssetFactories[sourceType]
            : Array.Empty<Type>();
        converters = converters.Append(converterType).ToArray();
        AssetFactories[sourceType] = converters;
        AssertFactoriesOrder[converterType] = (int)order;

        return this;
    }

    public VisualizationConfiguration RegisterContentType(string contentType, Type component)
    {
        ContentTypes[contentType] = component;

        return this;
    }
}
