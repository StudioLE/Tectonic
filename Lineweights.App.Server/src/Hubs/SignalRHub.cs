using Lineweights.Workflows.Assets;
using Lineweights.Workflows.Visualization;
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
    [HubMethodName(VisualizeInServerApp.ToHub)]
    public async Task SendToHub(Asset asset)
    {
        await Clients.All.SendAsync(VisualizeInServerApp.ToAllClients, asset);
    }
}
