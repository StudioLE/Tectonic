using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Shared;

public partial class SignalRComponent
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

    /// <inheritdoc cref="ResultsState"/>
    [Inject]
    protected ResultsState Results { get; set; } = default!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (SignalR.IsWebAssembly)
            return;

        // Connect SignalR
        SignalR.ConnectionUrl = NavigationManager.ToAbsoluteUri(SendToDashboard.HubPath);
        await SignalR.Connect();

        // Add to collection on receive
        SendToDashboard.OnReceiveFromHub(SignalR.Connection ?? throw new("SignalR connection was null"), Results.Collection.Add);
    }
}
