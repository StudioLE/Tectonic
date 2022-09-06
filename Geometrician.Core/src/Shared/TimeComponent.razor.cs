using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Geometrician.Core.Shared;

public class TimeComponentBase : ComponentBase, IDisposable
{
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(4);
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private Timer? _timer;

    [Inject]
    public ILogger<TimeComponent> Logger { get; set; } = default!;

    /// <summary>
    /// The <see cref="System.DateTime"/> to display.
    /// </summary>
    [Parameter]
    public DateTime DateTime { get; set; }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            RefreshAtIntervals();
    }

    /// <summary>
    /// Refresh this card at intervals to ensure the time since creation increases.
    /// </summary>
    private void RefreshAtIntervals()
    {
        _timer = new(OnInterval, null, _initialDelay, _interval);
    }

    private void OnInterval(object? _)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Logger.LogDebug($"{nameof(Dispose)} called. DateTime: {DateTime}");
        _timer?.Dispose();
    }
}
