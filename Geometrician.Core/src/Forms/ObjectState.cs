namespace Geometrician.Core.Forms;

public class ObjectState
{
    public string Title { get; }

    public IReadOnlyCollection<PropertyState> Properties { get; }

    public ObjectState(object instance)
    {
        Type type = instance.GetType();
        Title = type.Name;
        Properties = type
            .GetProperties()
            .Select(x => new PropertyState(instance, x))
            .ToArray();
    }
}
