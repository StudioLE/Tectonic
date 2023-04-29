using Cascade.Components.Shared;
using Geometrician.Core.Assets;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StudioLE.Core.Results;

namespace Cascade.Components.Visualization;

/// <summary>
/// A <see cref="IComponent"/> to defer the execution of a <see cref="Factory"/>.
/// If <see cref="Factory"/> has a <see cref="Success"/> result then the passed child content is rendered.
/// Otherwise a load button is rendered.
/// </summary>
public class DeferExecutionComponentBase : TemplatedComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<DeferExecutionComponentBase> Logger { get; set; } = default!;

    /// <summary>
    /// The factory to defer execution of.
    /// </summary>
    [Parameter]
    public IAssetFactory<IAsset> Factory { get; set; } = default!;

    /// <summary>
    /// A callback function to call after execution has completed.
    /// </summary>
    [Parameter]
    public Func<Task> AfterExecution { get; set; } = default!;

    /// <summary>
    /// Execute the factory.
    /// </summary>
    protected async Task Execute()
    {
        Logger.LogDebug($"{nameof(Execute)}() called on {Factory.GetType()}.");
        await Factory.Execute();
        await AfterExecution.Invoke();
    }
}
