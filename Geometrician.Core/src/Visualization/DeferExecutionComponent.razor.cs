using Geometrician.Core.Shared;
using Lineweights.Core.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Visualization;

public class DeferExecutionComponentBase : TemplatedComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<DeferExecutionComponentBase> Logger { get; set; } = default!;

    /// <inheritdoc cref="IAssetFactory{TResult}"/>
    [Parameter]
    public IAssetFactory<IAsset> Factory { get; set; } = default!;

    [Parameter]
    public Func<Task> AfterExecution { get; set; } = default!;

    protected async Task Execute()
    {
        Logger.LogDebug($"{nameof(Execute)}() called on {Factory.GetType()}.");
        await Factory.Execute();
        await AfterExecution.Invoke();
    }
}
