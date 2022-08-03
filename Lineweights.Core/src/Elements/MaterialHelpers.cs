using StudioLE.Core.System;

namespace Lineweights.Core.Elements;

/// <summary>
/// Methods to help with <see cref="Material"/>.
/// </summary>
public static class MaterialHelpers
{
    /// <summary>
    /// Create a material with the color from <see cref="ColorHelpers.ColorByName"/>.
    /// The <see cref="Element.Id"/> is derived using <see cref="GuidHelpers.Create(Guid, string)"/>
    /// to minimise repetition.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <returns>A new <see cref="Material"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Material MaterialByName(string name)
    {
        Color color = ColorHelpers.ColorByName(name);
        return new(name, color, id: GuidHelpers.Create(typeof(Colors).GUID, name));
    }
}
