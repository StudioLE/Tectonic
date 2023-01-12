using Lineweights.Core.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Components.Visualization;

public class ViewerComponentBase<TAsset> : ComponentBase where TAsset : IAsset
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<ViewerComponentBase<TAsset>> Logger { get; set; } = default!;

    /// <inheritdoc cref="IAssetFactory{TResult}"/>
    [Parameter]
    public IAssetFactory<TAsset> Factory { get; set; } = default!;

    protected virtual async Task AfterExecution()
    {
        await InvokeAsync(StateHasChanged);
    }
}
