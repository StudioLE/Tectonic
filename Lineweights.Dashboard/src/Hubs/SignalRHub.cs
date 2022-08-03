using Lineweights.Results;
using Microsoft.AspNetCore.SignalR;

namespace Lineweights.Dashboard.Hubs;

// TODO: Rename to SignalRHub

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
        Console.WriteLine($"{nameof(SignalRHub)} {SendToDashboard.ToHub} {result.Metadata.Id}");
        await Clients.All.SendAsync(SendToDashboard.ToAllClients, result);
    }
}
