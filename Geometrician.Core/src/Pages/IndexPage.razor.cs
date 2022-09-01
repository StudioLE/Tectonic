using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Pages;

/// <summary>
/// Code-behind the <see cref="IndexPage"/>.
/// </summary>
public class IndexPageBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<IndexPage> Logger { get; set; } = default!;

    /// <inheritdoc cref="NavigationManager"/>
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Logger.LogDebug($"{nameof(OnInitialized)}() called.");
        Navigation.NavigateTo("/run");
    }
}
