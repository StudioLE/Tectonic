using Lineweights.Workflows.Assets;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lineweights.Workflows.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> in <c>Geometrician.Server</c>.
/// Use an <see cref="AssetBuilder"/> to convert the <see cref="Model"/> to individual <see cref="Asset"/>.
/// Save the assets to blob storage and then send the assets via SignalR to be visualised by a remote
/// app such as <c>Geometrician.Server</c>.
/// </summary>
/// <remarks>
/// The <see cref="VisualizeInServerApp"/> is obtained via a
/// <see href="https://refactoring.guru/design-patterns/singleton">singleton pattern</see> so each
/// <see cref="IVisualizationStrategy"/> shares the same hub connection.
/// </remarks>
public sealed class VisualizeInServerApp : IVisualizationStrategy
{
    #region Constants

    /// <summary>
    /// The path of the results hub.
    /// </summary>
    public const string HubPath = "/signals";

    /// <summary>
    /// The full url of the results hub.
    /// </summary>
    public const string HubUrl = $"http://localhost:3000{HubPath}";

    /// <summary>
    /// The name of the hub method used to send to the hub.
    /// </summary>
    public const string HubMethod = "Asset";

    #endregion

    private readonly IStorageStrategy _storageStrategy = new BlobStorageStrategy();

    private readonly HubConnection _connection;

    internal HubConnectionState State => _connection.State;

    internal VisualizeInServerApp()
    {
        _connection = HubConnectionSingleton.GetInstance();
    }

    /// <inheritdoc cref="VisualizeInServerApp"/>
    public async Task<Asset> Execute(Model model, DocumentInformation doc)
    {
        // TODO: Add task cancellation if the Hub is disconnected.
        if (_connection.State == HubConnectionState.Disconnected)
        {
            Console.WriteLine("Failed to send to server. SignalR is disconnected.");
            return new()
            {
                Errors = new [] { "Failed to send to server. SignalR is disconnected." }
            };
        }
        AssetBuilder builder = AssetBuilder.Default(_storageStrategy, model, doc);
        Asset asset = await builder.Build();
        await _storageStrategy.RecursiveWriteContentToStorage(asset);
        try
        {
            await _connection.SendAsync(HubMethod, asset);
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
