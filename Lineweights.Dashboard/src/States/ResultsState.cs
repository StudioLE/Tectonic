using System.Collections.ObjectModel;
using Lineweights.Workflows.Results;

namespace Lineweights.Dashboard.States;

/// <summary>
/// The SignalR connection state.
/// </summary>
/// <remarks>
/// Follows the <see href="https://stackoverflow.com/a/56223698/247218">state</see> pattern.
/// </remarks>
public class ResultsState
{
    /// <summary>
    /// The available results.
    /// </summary>
    public ObservableCollection<Result> Collection { get; } = new();
}
