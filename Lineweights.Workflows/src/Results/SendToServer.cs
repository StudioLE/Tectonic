using System.IO;
using System.Text;
using Lineweights.Workflows.Containers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Patterns;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// An <see cref="IResultStrategy"/> to send a <see cref="Model"/> via SignalR to be visualised by a remote
/// app such as <c>Lineweights.App.Server</c>.
/// </summary>
/// <remarks>
/// The <see cref="HubConnection"/> is obtained via a
/// <see href="https://refactoring.guru/design-patterns/singleton">singleton pattern</see> so each
/// <see cref="SendToServer"/> shares the same hub connection.
/// </remarks>
public sealed class SendToServer : IResultStrategy
{
    #region Constants

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
    public const string ToHub = "SendToHub";

    /// <summary>
    /// The name of the hub method used to send to clients.
    /// </summary>
    public const string ToAllClients = "SendToAllClients";

    #endregion

    private readonly IStorageStrategy _storageStrategy = new BlobStorageStrategy();

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

    /// <inheritdoc cref="SendToServer"/>
    public async Task<Container> Execute(Model model, DocumentInformation doc)
    {
        // TODO: Add task cancellation if the Hub is disconnected.
        if (_connection.State == HubConnectionState.Disconnected)
        {
            Console.WriteLine("Failed to send to server. SignalR is disconnected.");
            return new()
            {
                Errors = new [] { "Failed to send to server. SignalR is disconnected." }
            };
        }
        ContainerBuilder builder = ContainerBuilder.Default(_storageStrategy, model, doc);
        Container container = await builder.Build();
        await RecursiveWriteContent(container);
        try
        {
            await _connection.SendAsync(ToHub, container);
        }
        catch (Exception e)
        {
            container.Errors = container
                .Errors
                .Concat(new[]
                {
                    "Failed to send to server.",
                    e.Message
                })
                .ToArray();
        }
        return container;
    }

    /// <summary>
    /// On receiving a <see cref="Container"/> .
    /// </summary>
    public static void OnReceiveFromHub(HubConnection connection, Action<Container> handler)
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

    private async Task RecursiveWriteContent(Container container)
    {
        if (container.Content is not null)
        {
            string fileName = container.Info.Id + (container.ContentType.GetExtensionByContentType() ?? ".txt");
            byte[] byteArray = Encoding.ASCII.GetBytes(container.Content);
            MemoryStream stream = new(byteArray);
            _ = await _storageStrategy.WriteAsync(container, fileName, stream);
        }
        foreach (Container child in container.Children)
            await RecursiveWriteContent(child);
    }
}
