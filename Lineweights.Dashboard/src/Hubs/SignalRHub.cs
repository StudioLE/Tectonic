using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.SignalR;

namespace Lineweights.Dashboard.Hubs;

/// <summary>
/// SignalR connection hub.
/// </summary>
public sealed class SignalRHub : Hub
{
    /// <summary>
    /// Send a message.
    /// </summary>
    [HubMethodName(SendToDashboard.ToHub)]
    public async Task SendToHub(Result result)
    {
        await Clients.All.SendAsync(SendToDashboard.ToAllClients, result);
    }
}
