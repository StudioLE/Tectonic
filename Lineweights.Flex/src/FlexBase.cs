using Lineweights.Flex.Coordination;
using Lineweights.Flex.Sequences;

namespace Lineweights.Flex;

/// <summary>
/// <para>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create <see cref="ElementInstance"/>
/// following a <see cref="SequenceBuilder"/> in the <see cref="_mainAxis"/>.
/// </para>
/// <para>
/// The logic follows the concepts of
/// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Flexible_Box_Layout/Basic_Concepts_of_Flexbox">flexbox</see>
/// which gives full control over the positioning of elements using <see cref="Justification"/> and <see cref="Alignment"/>.
/// </para>
/// <para>
/// Each item to be placed can have unique minimum <see cref="Spacing"/> set by <see cref="Proxy.MinSpacing"/> or
/// extracted from the <see cref="Element.AdditionalProperties"/> by <see cref="PropertyHelpers.GetProperty{T}(Element)"/>.
/// </para>
/// </summary>
public abstract class FlexBase
{
    #region Fields

    /// <summary>
    /// The orientation .
    /// </summary>
    private Transform _orientation = new();

    /// <summary>
    /// The main axis along which elements are repeated.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected internal Vector3 _mainAxis => _orientation.XAxis;

    /// <summary>
    /// The cross axis in which elements are aligned.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected internal Vector3 _crossAxis => _orientation.YAxis;

    /// <summary>
    /// The normal axis in which elements are aligned.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected internal Vector3 _normalAxis => _orientation.ZAxis;

    /// <summary>
    /// The strategy for distributing remaining space between elements on the main axis.
    /// </summary>
    /// <remarks>
    /// This is equivalent to the flexbox
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/justify-content">justify-content</see> property.
    /// </remarks>
    protected Justification _mainJustification = Justification.SpaceBetween;

    /// <summary>
    /// The strategy for aligning elements on the cross axis.
    /// </summary>
    /// <remarks>
    /// This is equivalent to the flexbox
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/align-items">align-items</see> property.
    /// </remarks>
    protected Alignment _crossAlignment = Alignment.Center;

    /// <summary>
    /// The strategy for aligning elements on the normal axis.
    /// </summary>
    /// <remarks>
    /// This is equivalent to the flexbox
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/align-items">align-items</see> property.
    /// </remarks>
    protected Alignment _normalAlignment = Alignment.Center;

    /// <summary>
    /// The strategy for aligning the setting out on the cross axis.
    /// </summary>
    /// <remarks>
    /// This is equivalent to the flexbox
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/align-self">align-self</see> property.
    /// </remarks>
    protected Alignment _crossSettingOut = Alignment.Center;

    /// <summary>
    /// The strategy for aligning the setting out point on the normal axis.
    /// </summary>
    /// <remarks>
    /// This is equivalent to the flexbox
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/align-self">align-self</see> property.
    /// </remarks>
    protected Alignment _normalSettingOut = Alignment.Center;

    #endregion

    #region Builder methods


    /// <inheritdoc cref="_orientation"/>
    protected FlexBase Orientation(Vector3 mainAxis, Vector3 crossAxis, Vector3 normalAxis)
    {
        mainAxis = mainAxis.Unitized();
        crossAxis = crossAxis.Unitized();

        if (!mainAxis.IsParallelTo(Vector3.XAxis)
            && !mainAxis.IsParallelTo(Vector3.YAxis)
            && !mainAxis.IsParallelTo(Vector3.ZAxis))
            throw new ArgumentException("Failed to set orientation. The main axis must be parallel to either the X, Y, or Z axis.");
        if (!crossAxis.IsParallelTo(Vector3.XAxis)
            && !crossAxis.IsParallelTo(Vector3.YAxis)
            && !crossAxis.IsParallelTo(Vector3.ZAxis))
            throw new ArgumentException("Failed to set orientation. The cross axis must be parallel to either the X, Y, or Z axis.");
        if (!normalAxis.IsParallelTo(Vector3.XAxis)
            && !normalAxis.IsParallelTo(Vector3.YAxis)
            && !normalAxis.IsParallelTo(Vector3.ZAxis))
            throw new ArgumentException("Failed to set orientation. The normal axis must be parallel to either the X, Y, or Z axis.");
        if (mainAxis.IsParallelTo(crossAxis))
            throw new ArgumentException("Failed to set orientation. The main axis can't be parallel to the cross axis.");
        if (mainAxis.IsParallelTo(normalAxis))
            throw new ArgumentException("Failed to set orientation. The main axis can't be parallel to the normal axis.");

        _orientation = new(Vector3.Origin, mainAxis, crossAxis, normalAxis);

        return this;
    }

    #endregion
}
