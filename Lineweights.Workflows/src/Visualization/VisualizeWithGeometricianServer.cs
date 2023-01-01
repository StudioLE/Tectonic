using System.Text;
using Elements.Serialization.JSON;
using Lineweights.Core.Documents;
using Lineweights.Core.Serialisation;
using Lineweights.Workflows.Documents;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> in <c>Geometrician.Server</c>.
/// Use an <see cref="AssetBuilder"/> to convert the <see cref="Model"/> to individual <see cref="Asset"/>.
/// Save the assets to blob storage and then send the assets via SignalR to be visualised by a remote
/// app such as <c>Geometrician.Server</c>.
/// </summary>
public sealed class VisualizeWithGeometricianServer : IVisualizationStrategy
{
    private readonly BlobStorageStrategy _blobStorage = new();
    private readonly IAssetBuilder _assetBuilder;
    private readonly HttpClient _httpClient;

    public VisualizeWithGeometricianServer(GeometricianService geometrician, IAssetBuilder assetBuilder)
    {
        _assetBuilder = assetBuilder;
        _assetBuilder.StorageStrategy = _blobStorage;
        _httpClient = geometrician.HttpClient;
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    public async Task<Asset> Execute(Model model, DocumentInformation doc)
    {
        Asset asset = await _assetBuilder.Build(model);
        // TODO: With POST methods we no longer need to write content to storage.
        await _blobStorage.RecursiveWriteContentToStorage(asset);
        try
        {
            JsonSerializerSettings settings = new()
            {
                ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
            };
            string json = JsonConvert.SerializeObject(asset, settings);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(GeometricianService.AssetUrl, content);
        }
        catch (TaskCanceledException)
        {
            // Do nothing.
        }
        catch (Exception e)
        {
            asset.Errors = asset
                .Errors
                .Concat(new[]
                {
                    "Failed to send to server.",
                    e.Message
                })
                .ToArray();
        }
        return asset;
    }
}
