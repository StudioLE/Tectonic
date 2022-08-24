using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Lineweights.Dashboard.Core.Shared;

public partial class ActivityRunnerComponent
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<SignalRComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="ActivityRunnerState"/>
    [Inject]
    protected ActivityRunnerState Runner { get; set; } = default!;
}
