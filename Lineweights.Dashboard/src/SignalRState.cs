using Microsoft.AspNetCore.SignalR.Client;

namespace Lineweights.Dashboard;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class SignalRState : IAsyncDisposable
{
    /// <summary>
    /// The URI of the connection hub.
    /// </summary>
    public Uri? ConnectionUrl { get; set; }

    /// <summary>
    /// The SignalR connection.
    /// </summary>
    public HubConnection? Connection { get; set; }

    /// <summary>
    /// Is SignalR connected?
    /// </summary>
    public bool IsConnected => Connection?.State == HubConnectionState.Connected;

    /// <summary>
    /// Connect to SignalR if disconnected.
    /// </summary>
    public async Task Connect()
    {
        Connection ??= new HubConnectionBuilder()
            .WithUrl(ConnectionUrl ?? throw new("Could not create connection. Url is not set."))
            .AddNewtonsoftJsonProtocol()
            .Build();

        if (Connection.State == HubConnectionState.Disconnected)
            await Connection.StartAsync();
    }

    /// <summary>
    /// Disconnect to SignalR if connected.
    /// </summary>
    public async Task Disconnect()
    {
        if (Connection is not null && Connection.State == HubConnectionState.Connected)
            await Connection.StopAsync();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Connection is not null)
            await Connection.DisposeAsync();
    }
}
