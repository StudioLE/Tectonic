using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.App.Core.Pages;

/// <summary>
/// Code-behind the <see cref="Index"/> page.
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
