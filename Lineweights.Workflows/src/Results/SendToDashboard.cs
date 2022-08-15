using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Patterns;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A <see cref="IResultStrategy"/> to send a <see cref="Model"/> to the dashboard as a <see cref="Result"/>.
/// The result is then visualised on the dashboard.
/// </summary>
/// <remarks>
/// The <see cref="HubConnection"/> is obtained via a
/// <see href="https://refactoring.guru/design-patterns/singleton">singleton pattern</see> so each
/// <see cref="SendToDashboard"/> shares the same hub connection.
/// </remarks>
public sealed class SendToDashboard : IResultStrategy
{
    #region Constants

    /// <summary>
    /// The path of the results hub.
    /// </summary>
    public const string HubPath = "/signals";

    /// <summary>
    /// The full url of the results hub.
    /// </summary>
    public const string HubUrl = $"http://localhost:5242{HubPath}";

    /// <summary>
    /// The name of the hub method used to send to the hub.
    /// </summary>
    public const string ToHub = "SendToHub";

    /// <summary>
    /// The name of the hub method used to send to clients.
    /// </summary>
    public const string ToAllClients = "SendToAllClients";

    #endregion

    private readonly HubConnection _connection = GetHubConnectionInstance();

    internal HubConnectionState State => _connection.State;

    internal static HubConnection GetHubConnectionInstance()
    {
        return Singleton<HubConnection>.GetInstance(() =>
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new NoRetries())
                .WithUrl(HubUrl)
                .AddNewtonsoftJsonProtocol()
                .Build();
            connection.StartAsync();
            return connection;
        });

    }

    /// <inheritdoc cref="SendToDashboard"/>
    public Result Execute(Model model, DocumentInformation doc)
    {
        if (_connection.State == HubConnectionState.Disconnected)
            throw new("Failed to SendToDashboard. The dashboard is disconnected.");
        Result result = ResultBuilder.Default(new BlobStorageStrategy(), model, doc);
        SendToHub(result);
        return result;
    }

    /// <summary>
    /// Send a <see cref="Result"/> from a client to the hub.
    /// </summary>
    private void SendToHub(Result result)
    {
        _connection.SendAsync(ToHub, result);
    }

    /// <summary>
    /// On receiving a <see cref="Result"/> .
    /// </summary>
    public static void OnReceiveFromHub(HubConnection connection, Action<Result> handler)
    {
        connection.On(ToAllClients, handler);
    }

    /// <inheritdoc cref="IRetryPolicy"/>
    private class NoRetries : IRetryPolicy
    {
        /// <inheritdoc />
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return null;
        }
    }
}
