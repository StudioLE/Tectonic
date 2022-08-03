namespace Lineweights.Flex;

/// <summary>
/// Define how space is distributed between and around elements.
/// </summary>
/// <remarks>
/// Based on flexbox <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/justify-content">justify-content</see>.
/// </remarks>
public enum Justification
{
    /// <summary>
    /// Elements are packed flush together at the start of the container.
    /// </summary>
    Start,

    /// <summary>
    /// Elements are packed flush together at the end of the container.
    /// </summary>
    End,

    /// <summary>
    /// Elements are packed flush together at the center of the container.
    /// </summary>
    Center,

    /// <summary>
    /// Elements are distributed evenly in the container.
    /// The spacing between each pair of adjacent items is the same.
    /// The empty space before the first and after the last item equals
    /// half of the space between each pair of adjacent items.
    /// </summary>
    SpaceAround,

    /// <summary>
    /// Elements are distributed evenly in the container.
    /// The spacing between each pair of adjacent items is the same.
    /// The first element is flush with the start of the container
    /// The last element is flush with the end of the container.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Elements are distributed evenly in the container.
    /// The spacing is exactly the same between:
    /// each pair of adjacent items;
    /// the first element and the start of the container;
    /// the last element and the end of the container.
    /// </summary>
    SpaceEvenly
}

/// <summary>
/// Define the alignment of elements.
/// </summary>
/// <remarks>
/// Based on flexbox <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/align-items">align-items</see>.
/// </remarks>
public enum Alignment
{
    /// <summary>
    /// Elements are packed flush together at the start of the container.
    /// </summary>
    Start,

    /// <summary>
    /// Elements are packed flush together at the end of the container.
    /// </summary>
    End,

    /// <summary>
    /// Elements are packed flush together at the center of the container.
    /// </summary>
    Center
}
