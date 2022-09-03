using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Lineweights.Workflows.Visualization;

public static class HubConnectionSingleton
{
    private static readonly object _lock = new();
    private static HubConnection? _instance;

    /// <summary>
    /// The method used to construct an instance if it does not exist.
    /// </summary>
    public static Func<HubConnection> CreateInstance { get; } = () =>
    {
        HubConnection connection = new HubConnectionBuilder()
            .WithAutomaticReconnect(new NoRetries())
            .WithUrl(VisualizeInServerApp.HubUrl)
            .AddNewtonsoftJsonProtocol()
            .Build();
        connection.StartAsync();
        return connection;
    };

    /// <summary>
    /// Get a singleton instance.
    /// </summary>
    public static HubConnection GetInstance()
    {
        if (_instance is not null)
            return _instance;

        if (CreateInstance is null)
            throw new("Failed to get singleton instance. CreateInstance is not set.");

        lock (_lock)
            _instance ??= CreateInstance.Invoke();

        return _instance;
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
