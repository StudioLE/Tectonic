using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using Lineweights.Results;

namespace Lineweights.Dashboard.Pages;

/// <summary>
/// Code-behind the index page.
/// </summary>
public class DashboardBase : ComponentBase, IDisposable
{
    /// <summary>
    /// Manage the URI navigation.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    /// <inheritdoc cref="SignalRState"/>
    [Inject]
    protected SignalRState SignalR { get; set; } = default!;

    /// <inheritdoc cref="TestRunnerState"/>
    [Inject]
    protected TestRunnerState Runner { get; set; } = default!;

    /// <summary>
    /// The available signals.
    /// </summary>
    public ObservableCollection<Result> Results { get; } = new();
    
    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        // Connect SignalR
        SignalR.ConnectionUrl = NavigationManager.ToAbsoluteUri(SendToDashboard.HubPath);
        await SignalR.Connect();

        // Bind the results collection
        Results.CollectionChanged += OnResultsChanged;
        SendToDashboard.OnReceiveFromHub(SignalR.Connection!, Results.Add);
    }

    /// <summary>
    /// Notify that the state has changed.
    /// </summary>
    private async void OnResultsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Results.CollectionChanged -= OnResultsChanged;
    }
}
