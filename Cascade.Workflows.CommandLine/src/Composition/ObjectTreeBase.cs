using System.Reflection;

namespace Cascade.Workflows.CommandLine.Composition;

public abstract class ObjectTreeBase
{
    public IReadOnlyCollection<ObjectTreeProperty> Properties { get; private set; } = Array.Empty<ObjectTreeProperty>();

    protected void SetProperties(Type type, ObjectTreeBase instance)
    {
        PropertyInfo[] childProperties = type.GetProperties();
        Properties = childProperties
            .Where(property => property.SetMethod is not null)
            .Select(property =>
            {
                string? parentFullKey = this is ObjectTreeProperty treeProperty
                    ? treeProperty.FullKey
                    : null;
                return new ObjectTreeProperty(property, instance, parentFullKey);
            })
            .ToArray();
    }

    public IEnumerable<ObjectTreeProperty> FlattenProperties()
    {
        return Properties
            .SelectMany(property => Array.Empty<ObjectTreeProperty>()
                .Append(property)
                .Concat(property.FlattenProperties()));
    }

}
