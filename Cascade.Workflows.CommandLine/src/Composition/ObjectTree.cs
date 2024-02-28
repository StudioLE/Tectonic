namespace Cascade.Workflows.CommandLine.Composition;

public class ObjectTree : IObjectTreeComponent
{
    /// <inheritdoc/>
    public Type Type { get; }

    /// <inheritdoc/>
    public bool IsNullable { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<ObjectTreeProperty> Properties { get; }

    /// <summary>
    /// The instance the <see cref="ObjectTree"/> represents.
    /// </summary>
    public object Instance { get; internal set; }

    /// <summary>
    /// Creates a new instance of <see cref="ObjectTree"/>.
    /// </summary>
    public ObjectTree(object instance)
    {
        Type type = instance.GetType();
        Type? underlyingType = Nullable.GetUnderlyingType(type);
        IsNullable = underlyingType is not null;
        Type = underlyingType ?? type;
        Instance = instance;
        Properties = ObjectTreeComponentHelpers.CreateProperties(this);
    }
}
