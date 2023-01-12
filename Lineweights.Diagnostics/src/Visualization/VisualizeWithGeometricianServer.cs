using System.Text;
using Geometrician.Core.Visualization;
using Newtonsoft.Json;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Lineweights.Diagnostics.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> in <c>Geometrician.Server</c>.
/// </summary>
public sealed class VisualizeWithGeometricianServer : IVisualizationStrategy
{
    private readonly HttpClient _httpClient;

    public VisualizeWithGeometricianServer(VisualizationConfiguration geometrician)
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
        // Task[] tasks = requests
        //     // .Select(x => _blobStorage.RecursiveWriteLocalFilesToStorage(x.Asset))
        //     .ToArray();
        // Task.WaitAll(tasks);

        IResult postResult = await TryPost(requests);
        if(postResult is not Success)
            Console.WriteLine(postResult.Errors.Prepend("Failed to post.").Join());
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
    private async Task<IResult> TryConnect()
    {
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(new(HttpMethod.Head, VisualizationConfiguration.VisualizeRoute));
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
            HttpResponseMessage response = await _httpClient.PostAsync(VisualizationConfiguration.VisualizeRoute, content);
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
