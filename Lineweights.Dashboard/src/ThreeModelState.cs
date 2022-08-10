using Microsoft.JSInterop;

namespace Lineweights.Dashboard;

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
public class ThreeModelState : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public ThreeModelState(IJSRuntime js)
    {
        _moduleTask = new (() => js.InvokeAsync<IJSObjectReference>("import", "./ThreeModelState.js").AsTask());
    }

    public async Task Initialize3D(string id)
    {
        IJSObjectReference module = await _moduleTask.Value;
        await module.InvokeVoidAsync("initialize3D", id);
    }

    public async Task LoadModel(string id, string uri)
    {
        IJSObjectReference module = await _moduleTask.Value;
        await module.InvokeVoidAsync("loadModel", id, uri);
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
