using System.Reflection;
using StudioLE.Core.System;

namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to help with colors.
/// </summary>
public static class ColorHelpers
{
    /// <summary>
    /// Get a <see cref="Color"/> from <see cref="Colors"/> by the name of its property.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <returns>The <see cref="Color"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Color ColorByName(string name)
    {
        PropertyInfo[] properties = typeof(Colors).GetProperties();
        PropertyInfo? property = properties.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        return (Color?)property?.GetValue(null) ?? throw new($"Failed to get color by name. {nameof(Colors)}.{name} does not exist.");
    }

    /// <summary>
    /// Get the hexadecimal string of <paramref name="this"/> prefixed with a #.
    /// </summary>
    public static string ToHex(this Color @this)
    {
        int r = (@this.Red * 255).RoundToInt();
        int g = (@this.Green * 255).RoundToInt();
        int b = (@this.Blue * 255).RoundToInt();
        return $"#{r:X2}{g:X2}{b:X2}";
    }
}
