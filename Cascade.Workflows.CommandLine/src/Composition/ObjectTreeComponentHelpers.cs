using System.Reflection;

namespace Cascade.Workflows.CommandLine.Composition;

/// <summary>
/// Methods to help with <see cref="IObjectTreeComponent"/>.
/// </summary>
public static class ObjectTreeComponentHelpers
{
    /// <summary>
    /// Recursively get the properties of <paramref name="component"/> and its children as a flattened enumerable.
    /// </summary>
    public static IEnumerable<ObjectTreeProperty> FlattenProperties(this IObjectTreeComponent component)
    {
        return component
            .Properties
            .SelectMany(property => Array.Empty<ObjectTreeProperty>()
                .Append(property)
                .Concat(property.FlattenProperties()));
    }

    internal static IReadOnlyCollection<ObjectTreeProperty> CreateProperties(IObjectTreeComponent @this)
    {
        return @this
            .Type
            .GetProperties()
            .Where(IsSupported)
            .Select(property => new ObjectTreeProperty(property, @this))
            .ToArray();
    }

    /// <summary>
    /// Is the property supported?
    /// </summary>
    private static bool IsSupported(PropertyInfo property)
    {
        return !property.IsIndexer();
    }

    /// <summary>
    /// Is the property an indexer?
    /// </summary>
    private static bool IsIndexer(this PropertyInfo @this)
    {
        return @this.GetIndexParameters().Length != 0;
    }
}
