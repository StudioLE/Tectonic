using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Patterns;

namespace Lineweights.Results;

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

    private const string HubProtocol = "http:";
    private const string HubHostName = "localhost";
    private const string HubPort = "5242";

    /// <summary>
    /// The path of the results hub.
    /// </summary>
    public const string HubPath = "/signals";

    /// <summary>
    /// The full url of the results hub.
    /// </summary>
    public const string HubUrl = $"{HubProtocol}//{HubHostName}:{HubPort}{HubPath}";

    /// <summary>
    /// The name of the hub method used to send to the hub.
    /// </summary>
    public const string ToHub = "SendToHub";

    /// <summary>
    /// The name of the hub method used to send to clients.
    /// </summary>
    public const string ToAllClients = "SendToAllClients";

    #endregion

    private readonly HubConnection _connection;

    internal HubConnectionState State => _connection.State;

    /// <inheritdoc cref="SendToDashboard"/>
    public SendToDashboard(string url = HubUrl)
    {
        _connection = Singleton<HubConnection>.GetInstance(() =>
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new NoRetries())
                .WithUrl(url)
                .AddNewtonsoftJsonProtocol()
                .Build();
            connection.StartAsync();
            return connection;
        });
    }

    /// <inheritdoc cref="SendToDashboard"/>
    public Result Execute(Model model, DocumentInformation metadata)
    {
        if (_connection.State == HubConnectionState.Disconnected)
            throw new("Failed to SendToDashboard. The dashboard is disconnected.");
        Result result = ResultBuilder.Default(model, metadata);
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
        public TimeSpan? NextRetryDelay(RetryContext retryContext) => null;
    }
}
