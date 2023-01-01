using System.Text;
using Ardalis.Result;
using Elements.Serialization.JSON;
using Lineweights.Core.Documents;
using Lineweights.Core.Serialisation;
using Lineweights.Workflows.Documents;
using Newtonsoft.Json;
using StudioLE.Core.System;

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
    public async Task Execute(Model model, DocumentInformation doc)
    {
        Result<bool> connectResult = await TryConnect();
        if (!connectResult.IsSuccess)
        {
            Console.WriteLine(connectResult.Errors.Join());
            return;
        }

        Asset asset = await _assetBuilder.Build(model);

        Result<bool> postResult = await TryPostAsset(asset);
        if(!postResult.IsSuccess)
            Console.WriteLine(postResult.Errors.Join());
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    private async Task<Result<bool>> TryConnect()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(new(HttpMethod.Head, GeometricianService.AssetUrl));
            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Result<bool>.Error($"{response.StatusCode}: {response.ReasonPhrase}");
        }
        catch (TaskCanceledException e)
        {
            return Result<bool>.Error("The task was cancelled.", e.Message);
        }
        catch (Exception e)
        {
            return Result<bool>.Error(e.Message);
        }
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    private async Task<Result<bool>> TryPostAsset(Asset asset)
    {
        try
        {
            JsonSerializerSettings settings = new()
            {
                ContractResolver = new IgnoreConverterResolver(typeof(JsonInheritanceConverter))
            };
            string json = JsonConvert.SerializeObject(asset, settings);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(GeometricianService.AssetUrl, content);
            return response.IsSuccessStatusCode
                ? Result<bool>.Success(true)
                : Result<bool>.Error($"{response.StatusCode}: {response.ReasonPhrase}");
        }
        catch (TaskCanceledException e)
        {
            return Result<bool>.Error("The task was cancelled.", e.Message);
        }
        catch (Exception e)
        {
            return Result<bool>.Error(e.Message);
        }
    }
}
