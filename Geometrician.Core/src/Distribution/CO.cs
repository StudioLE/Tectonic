namespace Geometrician.Core.Distribution;

/// <summary>
/// Methods to help with coordinating dimensions such as those used in brickwork.
/// </summary>
/// <remarks>
/// <see href="https://www.brick.org.uk/admin/resources/designing-to-brickwork-dimensions.pdf">PDF with more information on brickwork dimensions.</see>
/// </remarks>
// ReSharper disable once InconsistentNaming
public static class CO
{
    /// <summary>
    /// Get the core size without any spacing or joins.
    /// </summary>
    /// <remarks>
    /// For example: An opening in brickwork.
    /// <see href="https://www.brick.org.uk/admin/resources/designing-to-brickwork-dimensions.pdf">PDF with more information on brickwork dimensions.</see>
    /// </remarks>
    /// <param name="core">The core size of an element without any spacing.</param>
    /// <param name="spacing">The size of one spacing.</param>
    /// <returns>The core size without any spacing.</returns>
    public static double Minus(double core, double spacing = 0)
    {
        return core;
    }

    /// <summary>
    /// Get the core size + 1 spacing or join.
    /// </summary>
    /// <remarks>
    /// For example: A panel of brickwork with opposite return ends.
    /// <see href="https://www.brick.org.uk/admin/resources/designing-to-brickwork-dimensions.pdf">PDF with more information on brickwork dimensions.</see>
    /// </remarks>
    /// <param name="core">The core size of an element without any spacing.</param>
    /// <param name="spacing">The size of one spacing.</param>
    /// <returns>The core size without any spacing.</returns>
    public static double Nominal(double core, double spacing)
    {
        return core + spacing;
    }

    /// <summary>
    /// Get the core size + 2 spacing or joins.
    /// </summary>
    /// <remarks>
    /// For example: A panel or column of brickwork between openings.
    /// <see href="https://www.brick.org.uk/admin/resources/designing-to-brickwork-dimensions.pdf">PDF with more information on brickwork dimensions.</see>
    /// </remarks>
    /// <param name="core">The core size of an element without any spacing.</param>
    /// <param name="spacing">The size of one spacing.</param>
    /// <returns>The core size without any spacing.</returns>
    public static double Positive(double core, double spacing)
    {
        return core + spacing * 2;
    }
}
