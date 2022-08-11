using Microsoft.JSInterop;

namespace Lineweights.Dashboard.Scripts;

/// <summary>
/// The Three.js model state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
///
/// This class provides an example of how JavaScript functionality can be wrapped
/// in a .NET class for easy consumption. The associated JavaScript module is
/// loaded on demand when first needed.
///
/// This class can be registered as scoped DI service and then injected into Blazor
/// components for use.
/// </remarks>
public class ModelViewerFacade : IAsyncDisposable {

    private readonly IJSRuntime _js;

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public ModelViewerFacade(IJSRuntime js)
    {
        _js = js;
        _moduleTask = new (() => js.InvokeAsync<IJSObjectReference>("import", "./dist/bundle.js").AsTask());
    }

    public async Task Init(string id, string uri)
    {
        await _js.InvokeVoidAsync("ModelViewer.init", id, uri);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
