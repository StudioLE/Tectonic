using Lineweights.Workflows.Assets;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.SignalR;

namespace Lineweights.App.Server.Hubs;

/// <summary>
/// SignalR connection hub.
/// </summary>
public sealed class SignalRHub : Hub
{
    /// <summary>
    /// Send a message.
    /// </summary>
    [HubMethodName(SendToServer.ToHub)]
    public async Task SendToHub(Asset asset)
    {
        await Clients.All.SendAsync(SendToServer.ToAllClients, asset);
    }
}
