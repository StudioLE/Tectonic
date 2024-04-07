using System.Reflection;
using StudioLE.Serialization.Composition;

namespace Tectonic.Extensions.CommandLine;

public static class ObjectPropertyExtensions
{
    public static string ToLongOption(this string str)
    {
        return "--" + str.ToLower();
    }

    public static string ToArgument(this string str)
    {
        return str.ToLower();
    }

    public static bool HasArgumentAttribute(this ObjectProperty property)
    {
        return property.Property.GetCustomAttribute<ArgumentAttribute>() is not null;
    }
}
