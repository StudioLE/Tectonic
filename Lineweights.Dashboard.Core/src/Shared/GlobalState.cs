using System.Collections.ObjectModel;
using Lineweights.Workflows.Containers;

namespace Lineweights.Dashboard.Core.Shared;

/// <summary>
/// The global state of the application.
/// </summary>
public class GlobalState
{
    /// <summary>
    /// The containers.
    /// </summary>
    public ObservableCollection<Container> Containers { get; } = new();
}
