namespace Cascade.Components.Composition;

/// <summary>
/// The current state of the activity composition.
/// </summary>
public class CompositionState
{
    /// <summary>
    /// The key of the currently selected assembly.
    /// This is the assembly name without a .dll extension.
    /// </summary>
    public string SelectedAssemblyKey { get; set; } = string.Empty;

    /// <summary>
    /// The key of the currently selected activity.
    /// This is the activity name without an assembly prefix.
    /// </summary>
    public string SelectedActivityKey { get; set; } = string.Empty;

    /// <summary>
    /// Is the assembly set?
    /// </summary>
    public bool IsAssemblySet => !string.IsNullOrEmpty(SelectedAssemblyKey);

    /// <summary>
    /// Is the activity set?
    /// </summary>
    public bool IsActivitySet => IsAssemblySet && !string.IsNullOrEmpty(SelectedActivityKey);
}
