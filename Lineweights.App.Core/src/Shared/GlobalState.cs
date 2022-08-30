using System.Collections.ObjectModel;
using Lineweights.Workflows.Assets;

namespace Lineweights.App.Core.Shared;

/// <summary>
/// The global state of the application.
/// </summary>
public class GlobalState
{
    /// <summary>
    /// The <see cref="Asset"/> currently in state.
    /// </summary>
    public ObservableCollection<Asset> Assets { get; } = new();

    /// <summary>
    /// The <see cref="Message"/> currently in state.
    /// </summary>
    public ObservableCollection<Message> Messages { get; } = new();
}
