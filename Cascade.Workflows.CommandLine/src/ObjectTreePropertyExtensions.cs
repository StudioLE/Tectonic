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
}
