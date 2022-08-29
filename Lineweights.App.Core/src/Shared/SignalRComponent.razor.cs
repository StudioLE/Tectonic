using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.App.Core.Shared;

public class SignalRComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<SignalRComponent> Logger { get; set; } = default!;

    /// <summary>
    /// Manage the URI navigation.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    /// <inheritdoc cref="SignalRState"/>
    [Inject]
    protected SignalRState SignalR { get; set; } = default!;

    /// <inheritdoc cref="GlobalState"/>
    [Inject]
    protected GlobalState State { get; set; } = default!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (SignalR.IsWebAssembly)
            return;

        // Connect SignalR
        SignalR.ConnectionUrl = NavigationManager.ToAbsoluteUri(SendToServer.HubPath);
        await SignalR.Connect();

        // Add to collection on receive
        SendToServer.OnReceiveFromHub(SignalR.Connection ?? throw new("SignalR connection was null"), State.Containers.Add);
    }
}
