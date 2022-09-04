using Geometrician.Core.Shared;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.SignalR;

namespace Geometrician.Server.Hubs;

/// <summary>
/// SignalR connection hub.
/// </summary>
public sealed class SignalRHub : Hub
{
    private readonly ILogger<SignalRHub> _logger;
    private readonly AssetState _state;

    public SignalRHub(ILogger<SignalRHub> logger, AssetState state)
    {
        _logger = logger;
        _state = state;
    }

    /// <summary>
    /// This is method is called when the SignalRHub receives a message.
    /// </summary>
    [HubMethodName(GeometricianService.HubMethod)]
    public Task OnAssetReceived(Asset asset)
    {
        _logger.LogDebug($"{nameof(OnAssetReceived)} called.");
        _state.Assets.Add(asset);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task OnConnectedAsync()
    {
        _logger.LogDebug($"{nameof(OnConnectedAsync)} called.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string message = $"{nameof(OnDisconnectedAsync)} called.";
        if (exception is null)
            _logger.LogDebug(message);
        else
            _logger.LogError(exception, message);
        return Task.CompletedTask;
    }
}
