using System.ComponentModel;
using System.Reflection;

namespace Tectonic;

/// <summary>
/// Methods to help with <see cref="IActivity"/>.
/// </summary>
public static class ActivityHelpers
{
    /// <summary>
    /// Get the name of the activity from the <see cref="IActivityMetadata"/>
    /// or the <see cref="DisplayNameAttribute"/>
    /// or the type name.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The name.</returns>
    public static string GetName(this IActivity activity)
    {
        if (activity is IActivityMetadata metadata)
            return metadata.Name;
        Type type = activity.GetType();
        DisplayNameAttribute? attribute = type.GetCustomAttribute<DisplayNameAttribute>();
        return attribute?.DisplayName ?? type.Name;
    }

    /// <summary>
    /// Get the descriptions of the activity from the <see cref="IActivityMetadata"/>
    /// or the <see cref="DescriptionAttribute"/>
    /// or an empty string.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>The description.</returns>
    public static string GetDescription(this IActivity activity)
    {
        if (activity is IActivityMetadata metadata)
            return metadata.Description;
        Type type = activity.GetType();
        DescriptionAttribute? attribute = type.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? string.Empty;
    }
}
