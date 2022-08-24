using Lineweights.Dashboard.Core.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Pages;

/// <summary>
/// Code-behind the dashboard page.
/// </summary>
public class DashboardBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<DashboardBase> Logger { get; set; } = default!;

    /// <inheritdoc cref="ResultsState"/>
    [Inject]
    protected ResultsState Results { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Results.Collection.CollectionChanged += OnResultsChanged;
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
