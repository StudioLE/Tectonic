namespace Geometrician.Components.Composition;

public class InputPackProxy
{
    public string Title { get; }

    public IReadOnlyCollection<InputProxy> Inputs { get; }

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
