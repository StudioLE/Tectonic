using System.Collections.ObjectModel;
using Lineweights.Workflows.Assets;

namespace Lineweights.App.Core.Shared;

/// <summary>
/// The global state of the application.
/// </summary>
public class GlobalState
{
    /// <summary>
    /// The assets.
    /// </summary>
    public ObservableCollection<Asset> Assets { get; } = new();
}
