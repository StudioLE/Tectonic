using Lineweights.Dashboard.States;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.Components;

namespace Lineweights.Dashboard.Pages;

/// <summary>
/// Code-behind the dashboard page.
/// </summary>
public class DashboardBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<DashboardBase> Logger { get; set; } = default!;

    /// <summary>
    /// Manage the URI navigation.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    /// <inheritdoc cref="ResultsState"/>
    [Inject]
    protected ResultsState Results { get; set; } = default!;

    /// <inheritdoc cref="SignalRState"/>
    [Inject]
    protected SignalRState SignalR { get; set; } = default!;

    /// <inheritdoc cref="ActivityRunnerState"/>
    [Inject]
    protected ActivityRunnerState Runner { get; set; } = default!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        // Connect SignalR
        SignalR.ConnectionUrl = NavigationManager.ToAbsoluteUri(SendToDashboard.HubPath);
        await SignalR.Connect();

        // Bind the results collection
        Results.Collection.CollectionChanged += OnResultsChanged;
        SendToDashboard.OnReceiveFromHub(SignalR.Connection!, Results.Collection.Add);
    }

    /// <summary>
    /// Notify that the state has changed.
    /// </summary>
    private async void OnResultsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Logger.LogDebug($"{nameof(OnResultsChanged)}() called.");
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Results.Collection.CollectionChanged -= OnResultsChanged;
    }
}
