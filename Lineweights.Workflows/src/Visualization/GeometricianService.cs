using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Lineweights.Workflows.Visualization;

public class GeometricianService
{
    /// <summary>
    /// The path of the results hub.
    /// </summary>
    public const string HubPath = "/signals";

    /// <summary>
    /// The full url of the results hub.
    /// </summary>
    public const string HubUrl = $"http://localhost:3000{HubPath}";

    /// <summary>
    /// The name of the hub method used to send to the hub.
    /// </summary>
    public const string HubMethod = "Asset";

    public HubConnection Connection { get; }

    public GeometricianService()
    {
        Connection = new HubConnectionBuilder()
            .WithAutomaticReconnect(new NoRetries())
            .WithUrl(HubUrl)
            .AddNewtonsoftJsonProtocol()
            .Build();
        Connection.StartAsync();
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
