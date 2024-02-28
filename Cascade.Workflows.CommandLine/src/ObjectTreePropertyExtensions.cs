using System.Reflection;
using Cascade.Workflows.CommandLine.Composition;

namespace Cascade.Workflows.CommandLine;

public static class ObjectTreePropertyExtensions
{
    public static string ToLongOption(this string str)
    {
        return "--" + str.ToLower();
    }

    public static string ToArgument(this string str)
    {
        return str.ToLower();
    }

    public static bool HasArgumentAttribute(this ObjectTreeProperty tree)
    {
        return tree.Property.GetCustomAttribute<ArgumentAttribute>() is not null;
    }

    public static IEnumerable<ObjectTreeProperty> FlattenProperties(this IObjectTreeComponent component)
    {
        return component
            .Properties
            .SelectMany(property => Array.Empty<ObjectTreeProperty>()
                .Append(property)
                .Concat(property.FlattenProperties()));
    }
}
