using Lineweights.Core.Distribution;
using Lineweights.Flex.Sequences;
using StudioLE.Core.Exceptions;

namespace Lineweights.Flex;

/// <inheritdoc cref="FlexBase"/>
public sealed class Flex2d : FlexBase
{
    #region Fields

    private bool _isBuilt = false;

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
    private IReadOnlyCollection<ISequenceBuilder> _mainSequences = Array.Empty<ISequenceBuilder>();

    /// <summary>
    /// The strategy for repeating elements along the cross axis.
    /// </summary>
    /// <remarks>
    /// <see cref="SequenceBuilder.Body"/> is set to the assembly created by <see cref="_mainSequences"/>.
    /// </remarks>
    private ISequenceBuilder _crossSequence = new SequenceBuilder();

    #endregion

    #region Properties

    internal IReadOnlyCollection<ElementInstance> Assemblies { get; private set; } = Array.Empty<ElementInstance>();

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

    /// <inheritdoc cref="_mainSequences"/>
    public Flex2d MainSequence(params ISequenceBuilder[] sequences)
    {
        _mainSequences = sequences;
        return this;
    }

    /// <inheritdoc cref="_crossSequence"/>
    public Flex2d CrossSequence(ISequenceBuilder sequence)
    {
        _crossSequence = sequence;
        return this;
    }

    #endregion

    #region Execution methods

    /// <summary>
    /// Execute the builder logic.
    /// </summary>
    public IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> Build()
    {
        if (_isBuilt)
            throw new("Failed to build. Build can only be called once.");
        if (_container is null)
            throw new($"Failed to build {nameof(Flex2d)}. Container is not set.");

        IReadOnlyCollection<ElementInstance> assemblies = DistributeInMainAxis();

        _crossSequence.Body(assemblies.ToArray());

        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> components = DistributeInCrossAxis();

        // TODO: You'll now need to transform to the setting out point.

        _isBuilt = true;
        return components;
    }

    private IReadOnlyCollection<ElementInstance> DistributeInMainAxis()
    {
        Flex1d mainBuilder = new Flex1d()
            .Bounds(_mainAxis, _crossAxis, _normalAxis, _mainAxis.Dimension(_container!.Bounds))
            .MainJustification(_mainJustification)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            .CrossAlignment(_crossAlignment)
            .NormalAlignment(_normalAlignment);

        ISequenceBuilder[] split = _mainSequences
            .SelectMany(sequence => sequence
                .Context(mainBuilder)
                .MaxLengthConstraint()
                .SplitWrapping())
            .ToArray();

        IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> sequences = split
            .Select(sequence => mainBuilder
                .Sequence(sequence)
                .Build())
            .ToArray();

        IReadOnlyCollection<ElementInstance> assemblies = sequences
            .Select(elements => CreateAssembly(mainBuilder, elements))
            .ToArray();

        return assemblies;
    }

    private IReadOnlyCollection<IReadOnlyCollection<ElementInstance>> DistributeInCrossAxis()
    {
        Flex1d crossBuilder = new Flex1d()
            .Bounds(_crossAxis, _mainAxis, _normalAxis, _crossAxis.Dimension(_container!.Bounds))
            .MainJustification(_crossJustification)
            .CrossSettingOut(Alignment.Center)
            .NormalSettingOut(Alignment.Center)
            //.SetCrossAlignment(CrossAlignment)
            //.SetNormalAlignment(NormalAlignment)
            .Sequence(_crossSequence);

        Assemblies = crossBuilder.Build();
        return Assemblies
            .Select(x => x.ToComponents())
            .ToArray();

    }

    private ElementInstance CreateAssembly(Flex1d builder, IReadOnlyCollection<Element> elements)
    {
        ElementInstance[] instances = elements
            .Select(element => element switch
            {
                // TODO: We should limit Sequence to only returning GeometricElement. Then we create instances somewhere appropriate (not here)
                GeometricElement geometric => geometric.CreateInstance(),
                ElementInstance instance => instance.BaseDefinition.CreateInstance(instance.Transform, string.Empty),
                _ => throw new TypeSwitchException<Element>("Failed to create instance.", element)
            })
            .ToArray();

        if (!instances.Any())
            throw new("Failed to create assembly. There were no instances.");

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
        double assemblyNormalSize = builder._normalAxis.Dimension(instanceBounds);
        double assemblyCrossSize = builder._crossAxis.Dimension(instanceBounds);
        BBox3 assemblyBounds = GetAssemblyBounds(builder, containerNormalSize, assemblyCrossSize);

        Assembly assembly = new(instances, assemblyBounds);
        Vector3 containerOrigin = _container!.Bounds.PointAt(0.5, 0.5, 0.5);

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

        // TODO: This isn't a reliable way to calculate the spacing.
        Spacing spacing = new()
        {
            X = builder._proxies.Max(x => x.MinSpacing.X),
            Y = builder._proxies.Max(x => x.MinSpacing.Y),
            Z = builder._proxies.Max(x => x.MinSpacing.Z)
        };
        assembly.SetProperty(spacing);

        // Note: We have changed behaviour. We are no longer setting the transform of the assembly as the instances are no longer moved.
        return assembly.CreateInstance("Assembly");
    }

    #endregion

    private static BBox3 GetAssemblyBounds(Flex1d builder, double containerNormalSize, double crossSize)
    {
        Vector3 mainMin = builder._mainAxis * builder.TargetLength * 0.5 * -1;
        Vector3 mainMax = builder._mainAxis * builder.TargetLength * 0.5;
        Vector3 normalMin = builder._normalAxis * containerNormalSize * 0.5 * -1;
        Vector3 normalMax = builder._normalAxis * containerNormalSize * 0.5;
        Vector3 crossMin = builder._crossAxis * crossSize * 0.5 * -1;
        Vector3 crossMax = builder._crossAxis * crossSize * 0.5;
        var vertices = new[]
        {
            mainMin,
            mainMax,
            crossMin,
            crossMax,
            normalMin,
            normalMax
        };
        return new(vertices);
    }
}

