using System.Text;
using Ardalis.Result;
using Lineweights.Core.Documents;
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
    private readonly HttpClient _httpClient;

    public VisualizeWithGeometricianServer(GeometricianService geometrician)
    {
        _httpClient = geometrician.HttpClient;
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    public async Task Execute(VisualizeRequest request)
    {
        Result<bool> connectResult = await TryConnect();
        if (!connectResult.IsSuccess)
        {
            Console.WriteLine(connectResult.Errors.Prepend("Failed to connect.").Join());
            return;
        }

        // TODO: Cancel the following tasks if try connect fails.
        Task task = _blobStorage.RecursiveWriteLocalFilesToStorage(request.Asset);
        task.Wait();
        Result<bool> postResult = await TryPost(request);
        if(!postResult.IsSuccess)
            Console.WriteLine(postResult.Errors.Prepend("Failed to post.").Join());
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    private async Task<Result<bool>> TryConnect()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(new(HttpMethod.Head, GeometricianService.VisualizeRoute));
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
    private async Task<Result<bool>> TryPost(VisualizeRequest request)
    {
        try
        {
            VisualizeRequestConverter converter = new();
            string json = JsonConvert.SerializeObject(request, converter);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(GeometricianService.VisualizeRoute, content);
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
