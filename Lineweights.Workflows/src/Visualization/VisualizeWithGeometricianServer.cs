using System.Text;
using StudioLE.Core.Results;
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
    public async Task Execute(params VisualizeRequest[] requests)
    {
        IResult connectResult = await TryConnect();
        if (connectResult is not Success)
        {
            Console.WriteLine(connectResult.Errors.Prepend("Failed to connect.").Join());
            return;
        }

        // TODO: Cancel the following tasks if try connect fails.
        Task[] tasks = requests
            .Select(x => _blobStorage.RecursiveWriteLocalFilesToStorage(x.Asset))
            .ToArray();
        Task.WaitAll(tasks);

        IResult postResult = await TryPost(requests);
        if(postResult is not Success)
            Console.WriteLine(postResult.Errors.Prepend("Failed to post.").Join());
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    private async Task<IResult> TryConnect()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(new(HttpMethod.Head, GeometricianService.VisualizeRoute));
            return response.IsSuccessStatusCode
                ? new Success()
                : new Failure($"{response.StatusCode}: {response.ReasonPhrase}");
        }
        catch (TaskCanceledException e)
        {
            return new Failure("The task was cancelled.", e);
        }
        catch (Exception e)
        {
            return new Failure(e);
        }
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    private async Task<IResult> TryPost(VisualizeRequest[] requests)
    {
        try
        {
            VisualizeRequestConverter converter = new();
            string json = JsonConvert.SerializeObject(requests, converter);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(GeometricianService.VisualizeRoute, content);
            return response.IsSuccessStatusCode
                ? new Success()
                : new Failure($"{response.StatusCode}: {response.ReasonPhrase}");
        }
        catch (TaskCanceledException e)
        {
            return new Failure("The task was cancelled.", e.Message);
        }
        catch (Exception e)
        {
            return new Failure(e.Message);
        }
    }
}
