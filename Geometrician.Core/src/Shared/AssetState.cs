using System.Collections.ObjectModel;
using Lineweights.Workflows.Assets;

namespace Geometrician.Core.Shared;

/// <summary>
/// The global state of the application.
/// </summary>
public class AssetState
{
    /// <summary>
    /// The <see cref="Asset"/> currently in state.
    /// </summary>
    public ObservableCollection<Asset> Assets { get; } = new();
}
