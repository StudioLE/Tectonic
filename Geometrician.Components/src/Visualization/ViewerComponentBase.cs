using Lineweights.Core.Assets;
using Microsoft.AspNetCore.Components;

namespace Geometrician.Components.Visualization;

/// <summary>
/// An abstract base <see cref="IComponent"/> including a <see cref="Factory"/> and a
/// virtual <see cref="AfterExecution"/> method which notifies of state change.
/// </summary>
/// <typeparam name="TAsset">The type of <see cref="IAsset"/>.</typeparam>
public class ViewerComponentBase<TAsset> : ComponentBase where TAsset : IAsset
{
    /// <inheritdoc cref="IAssetFactory{TResult}"/>
    [Parameter]
    public IAssetFactory<TAsset> Factory { get; set; } = default!;

    /// <summary>
    /// A virtual method to be called after the <see cref="Factory"/> is executed.
    /// </summary>
    protected virtual async Task AfterExecution()
    {
        await InvokeAsync(StateHasChanged);
    }
}
