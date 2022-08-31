using Microsoft.AspNetCore.Components;

namespace Geometrician.Core.Shared;

public class TimeComponentBase : ComponentBase
{
    /// <summary>
    /// The TimeSpan.
    /// </summary>
    [Parameter]
    public DateTime DateTime { get; set; }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        RefreshAtIntervals();
    }

    /// <summary>
    /// Refresh this card at intervals to ensure the time since creation increases.
    /// </summary>
    private void RefreshAtIntervals()
    {
        TimeSpan interval = TimeSpan.FromSeconds(5);
        Timer timer = new(_ => InvokeAsync(StateHasChanged), null, interval, interval);
    }
}
