using Lineweights.Core.Distribution;
using Lineweights.Flex.Sequences;
using StudioLE.Core.Exceptions;

namespace Lineweights.Flex;

/// <inheritdoc cref="FlexBase"/>
public sealed class Flex2d : FlexBase
{
    #region Fields

    private Flex1d _builder = new();

    /// <summary>
    /// The container relative to which elements are positioned.
    /// </summary>
    private Proxy? _container;

    /// <summary>
    /// The strategy for distributing remaining space between elements on the cross axis.
    /// </summary>
    /// <remarks>
    /// This is equivalent to the flexbox
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/CSS/justify-content">justify-content</see> property.
    /// </remarks>
    private Justification _crossJustification = Justification.Start;

    /// <summary>
    /// The strategies for repeating elements along the main axis.
    /// </summary>
    private IReadOnlyCollection<SequenceBuilder> _mainPatterns = Array.Empty<SequenceBuilder>();

    /// <summary>
    /// The strategy for repeating elements along the cross axis.
    /// </summary>
    /// <remarks>
    /// <see cref="SequenceBuilder._items"/> is set to the assembly created by <see cref="_mainPatterns"/>.
    /// </remarks>
    private SequenceBuilder _crossSequence = NonRepeatingSequence.WithoutOverflow();

    #endregion

    #region Builder methods

    /// <inheritdoc cref="_container"/>
    public Flex2d Container(GeometricElement container)
    {
        _container = new(container);
        return this;
    }

    /// <inheritdoc cref="FlexBase.Orientation(Vector3, Vector3, Vector3)"/>
    public new Flex2d Orientation(Vector3 mainAxis, Vector3 crossAxis, Vector3 normalAxis)
    {
        base.Orientation(mainAxis, crossAxis, normalAxis);
        return this;
    }

    /// <inheritdoc cref="FlexBase._mainJustification"/>
    public Flex2d MainJustification(Justification justification)
    {
        _mainJustification = justification;
        return this;
    }

    /// <inheritdoc cref="_crossJustification"/>
    public Flex2d CrossJustification(Justification justification)
    {
        _crossJustification = justification;
        return this;
    }

    /// <inheritdoc cref="FlexBase._crossAlignment"/>
    public Flex2d CrossAlignment(Alignment alignment)
    {
        _crossAlignment = alignment;
        return this;
    }

    /// <inheritdoc cref="FlexBase._normalAlignment"/>
    public Flex2d NormalAlignment(Alignment alignment)
    {
        _normalAlignment = alignment;
        return this;
    }

    ///// <inheritdoc cref="FlexBase._crossSettingOut"/>
    //public Flex2d CrossSettingOut(Alignment settingOut)
    //{
    //    _crossSettingOut = settingOut;
    //    return this;
    //}

    /// <inheritdoc cref="FlexBase._normalSettingOut"/>
    public Flex2d NormalSettingOut(Alignment settingOut)
    {
        _normalSettingOut = settingOut;
        return this;
    }

    /// <inheritdoc cref="_mainPatterns"/>
    public Flex2d MainPatterns(params SequenceBuilder[] patterns)
    {
        _mainPatterns = patterns;
        return this;
    }

    /// <inheritdoc cref="_crossSequence"/>
    public Flex2d CrossPattern(SequenceBuilder sequence)
    {
        _crossSequence = sequence;
        return this;
    }

    #endregion

    #region Execution methods

    /// <summary>
    /// Execute the builder.
    /// </summary>
    private Flex2d Build()
    {
        if (_container is null)
            throw new($"Failed to build {nameof(Flex2d)}. Container is not set.");

        // Distribute in the main axis
        _builder = new Flex1d()
            .Bounds(_mainAxis, _crossAxis, _normalAxis, _mainAxis.Dimension(_container.Bounds))
            .MainJustification(_mainJustification)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            .CrossAlignment(_crossAlignment)
            .NormalAlignment(_normalAlignment);

        IReadOnlyCollection<ElementInstance> assemblies1d = _mainPatterns
            .SelectMany(pattern => pattern is WrappingSequence wrapping
                ? wrapping.Split(_builder)
                : new[] { pattern })
            .Select(pattern1d => ToAssembly(_builder.Pattern(pattern1d), true))
            .ToArray();

        _crossSequence.Items(assemblies1d.ToArray());

        // Distribute in the cross axis
        _builder = new Flex1d()
            .Bounds(_crossAxis, _mainAxis, _normalAxis, _crossAxis.Dimension(_container.Bounds))
            .MainJustification(_crossJustification)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            //.SetCrossAlignment(CrossAlignment)
            //.SetNormalAlignment(NormalAlignment)
            .Pattern(_crossSequence);

        // TODO: You'll now need to transform to the setting out point.

        return this;
    }

    /// <summary>
    /// Execute the builder and create an instance of the results placed along the curve from the setting out point.
    /// </summary>
    internal IEnumerable<ElementInstance> ToAssemblyInstances()
    {
        Build();
        return _builder.ToComponents();
    }

    /// <summary>
    /// Execute the builder and create an assembly of each of the element instances.
    /// </summary>
    /// <remarks>
    /// An <see cref="Assembly"/> definition is created at <see cref="Vector3.Origin"/> and then instantiated
    /// moved to the correct relative location.
    /// </remarks>
    internal ElementInstance ToAssembly()
    {
        Build();
        return ToAssembly(_builder, false);
    }

    /// <summary>
    /// Execute the builder and create an assembly of each of the element instances.
    /// </summary>
    /// <remarks>
    /// An <see cref="Assembly"/> definition is created at <see cref="Vector3.Origin"/> and then instantiated
    /// moved to the correct relative location.
    /// </remarks>
    private ElementInstance ToAssembly(Flex1d builder, bool isMainArrangement)
    {
        IReadOnlyCollection<ElementInstance> instances = builder.ToComponents();

        Line line = Validate.IsTypeOrThrow<Line>(builder._curve, $"Failed to convert {nameof(Flex2d)} to assembly.");
        if (!line.PointAt(0.5).IsAlmostEqualTo(Vector3.Origin))
            throw new($"Failed to convert {nameof(Flex2d)} to assembly. Expected line to be at origin.");

        BBox3 instanceBounds = instances
            .Select(x => x.TransformedBounds())
            .Merged();

        // Determine the theoretical assembly bounds.
        // Be aware that the instances may project outside of the assembly bounds
        // This occurs with RepeatingPattern.WithOverflow
        double containerNormalSize = builder._normalAxis.Dimension(_container!.Bounds);
        double containerCrossSize = builder._crossAxis.Dimension(_container!.Bounds);
        double assemblyNormalSize = builder._normalAxis.Dimension(instanceBounds);
        double assemblyCrossSize = builder._crossAxis.Dimension(instanceBounds);
        Vector3 normalMin = builder._normalAxis * containerNormalSize * 0.5 * -1;
        Vector3 normalMax = builder._normalAxis * containerNormalSize * 0.5;
        Vector3 mainMin = builder._mainAxis * builder.TargetLength * 0.5 * -1;
        Vector3 mainMax = builder._mainAxis * builder.TargetLength * 0.5;
        Vector3[] assemblyVertices;
        if (isMainArrangement)
        {
            Vector3 crossMin = builder._crossAxis * assemblyCrossSize * 0.5 * -1;
            Vector3 crossMax = builder._crossAxis * assemblyCrossSize * 0.5;
            assemblyVertices = new[]
            {
                mainMin,
                mainMax,
                crossMin,
                crossMax,
                normalMin,
                normalMax,
            };
        }
        else
        {
            Vector3 crossMin = builder._crossAxis * containerCrossSize * 0.5 * -1;
            Vector3 crossMax = builder._crossAxis * containerCrossSize * 0.5;
            assemblyVertices = new[]
            {
                mainMin,
                mainMax,
                crossMin,
                crossMax,
                normalMin,
                normalMax
            };
        }
        BBox3 assemblyBounds = new(assemblyVertices);

        Assembly assembly = new(instances, assemblyBounds);
        Vector3 containerOrigin = _container!.Bounds.PointAt(0.5, 0.5, 0.5);

        if (isMainArrangement)
        {
            Vector3 settingOut = _normalSettingOut switch
            {
                Alignment.Start => containerOrigin
                                   - _normalAxis * containerNormalSize * 0.5
                                   + _normalAxis * assemblyNormalSize * 0.5,
                Alignment.End => containerOrigin
                                 + _normalAxis * containerNormalSize * 0.5
                                 - _normalAxis * assemblyNormalSize * 0.5,
                Alignment.Center => containerOrigin,
                _ => throw new EnumSwitchException<Alignment>("Failed to get normal setting out.", _normalSettingOut)
            };
            foreach (ElementInstance instance in instances)
                instance.Transform.Move(settingOut);
        }

        // TODO: This isn't a reliable way to calculate the spacing.
        Spacing spacing = new()
        {
            X = builder._results.Max(x => x.MinSpacing.X),
            Y = builder._results.Max(x => x.MinSpacing.Y),
            Z = builder._results.Max(x => x.MinSpacing.Z)
        };
        assembly.SetProperty(spacing);

        // Note: We have changed behaviour. We are no longer setting the transform of the assembly as the instances are no longer moved.
        return assembly.CreateInstance("Assembly");
    }

    /// <summary>
    /// Execute <see cref="Build()"/> and create components of the results.
    /// </summary>
    public IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> ToComponents()
    {
        ElementInstance assembly = ToAssembly();
        return assembly.ToComponents()
            .Select(x => x.ToComponents())
            .ToArray();
    }

    #endregion
}

