namespace Geometrician.Cascade.Components.Composition;

/// <summary>
/// A representation of an activity inputs class.
/// </summary>
public class InputPackProxy
{
    /// <summary>
    /// The title of the inputs.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Representations of each property of the activity input class.
    /// </summary>
    public IReadOnlyCollection<InputProxy> Inputs { get; }

    /// <see cref="InputPackProxy"/>
    public InputPackProxy(object inputPackInstance)
    {
        Type type = inputPackInstance.GetType();
        Title = type.Name;
        Inputs = type
            .GetProperties()
            .Select(x => new InputProxy(inputPackInstance, x))
            .ToArray();
    }
}
