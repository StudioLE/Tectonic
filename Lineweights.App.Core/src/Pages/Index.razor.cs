using Lineweights.App.Core.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.App.Core.Pages;

/// <summary>
/// Code-behind the <see cref="Index"/> page.
/// </summary>
public class IndexBase : ComponentBase, IDisposable
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<IndexBase> Logger { get; set; } = default!;

    /// <inheritdoc cref="GlobalState"/>
    [Inject]
    protected GlobalState State { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        State.Assets.CollectionChanged += OnAssetsChanged;
    }

    /// <summary>
    /// Notify that the state has changed.
    /// </summary>
    private async void OnAssetsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Logger.LogDebug($"{nameof(OnAssetsChanged)}() called.");
        await InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        State.Assets.CollectionChanged -= OnAssetsChanged;
    }
}
