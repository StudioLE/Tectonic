using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Lineweights.Dashboard.Core.Shared;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class SignalRState : IAsyncDisposable
{
    private readonly IJSRuntime _js;

    public SignalRState(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// The URI of the connection hub.
    /// </summary>
    public Uri? ConnectionUrl { get; set; }

    /// <summary>
    /// The SignalR connection.
    /// </summary>
    public HubConnection? Connection { get; set; }

    /// <summary>
    /// Is Blazor running as a web assembly.
    /// </summary>
    public bool IsWebAssembly => _js is IJSInProcessRuntime;

    /// <summary>
    /// Is SignalR connected?
    /// </summary>
    public bool IsConnected => Connection?.State == HubConnectionState.Connected;


    /// <summary>
    /// Connect to SignalR if disconnected.
    /// </summary>
    public async Task Connect()
    {
        if(IsWebAssembly)
            return;

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
