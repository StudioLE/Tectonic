using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lineweights.Workflows.Visualization;

/// <summary>
/// Visualize a <see cref="Model"/> in <c>Geometrician.Server</c>.
/// Use an <see cref="AssetBuilder"/> to convert the <see cref="Model"/> to individual <see cref="Asset"/>.
/// Save the assets to blob storage and then send the assets via SignalR to be visualised by a remote
/// app such as <c>Geometrician.Server</c>.
/// </summary>
public sealed class VisualizeWithGeometricianServer : IVisualizationStrategy
{
    private readonly HubConnection _connection;
    private readonly BlobStorageStrategy _blobStorage = new();
    private readonly IAssetBuilder _assetBuilder;

    internal HubConnectionState State => _connection.State;

    public VisualizeWithGeometricianServer(GeometricianService geometrician, IAssetBuilder assetBuilder)
    {
        _connection = geometrician.Connection;
        _assetBuilder = assetBuilder;
        _assetBuilder.StorageStrategy(_blobStorage);
    }

    /// <inheritdoc cref="VisualizeWithGeometricianServer"/>
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
        Asset asset = await _assetBuilder.Build(model);
        await _blobStorage.RecursiveWriteContentToStorage(asset);
        try
        {
            await _connection.SendAsync(GeometricianService.HubMethod, asset);
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
