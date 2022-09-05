using Lineweights.Core.Distribution;
using Lineweights.Flex.Sequences;
using StudioLE.Core.Exceptions;

namespace Lineweights.Flex;

/// <inheritdoc cref="FlexBase"/>
public sealed class Flex1d : FlexBase
{
    #region Fields

    private bool _isBoundsSet = false;

    /// <summary>
    /// The curve.
    /// </summary>
    internal Curve _curve = new Line();

    /// <summary>
    /// The target length.
    /// </summary>
    // TODO: Split into JustificationLength
    public double TargetLength => _curve.Length();

    /// <summary>
    /// The strategy for repeating elements along the main axis.
    /// </summary>
    private SequenceBuilder? _pattern;

    /// <summary>
    /// The elements created by executing <see cref="Build()"/>.
    /// </summary>
    internal IReadOnlyList<Proxy> _results = Array.Empty<Proxy>();

    #endregion

    #region Builder methods

    /// <summary>
    /// Set the container bounds.
    /// </summary>
    public Flex1d Bounds(Line line, Vector3 crossAxis, Vector3 normalAxis)
    {
        _isBoundsSet = true;
        Orientation(line.Direction(), crossAxis, normalAxis);
        _curve = line;
        return this;
    }

    /// <summary>
    /// Set the container bounds.
    /// </summary>
    public Flex1d Bounds(Line line, Vector3 crossAxis)
    {
        Vector3 normalAxis = line.Direction().Cross(crossAxis);
        Bounds(line, crossAxis, normalAxis);
        return this;
    }

    /// <summary>
    /// Set the container bounds.
    /// </summary>
    public Flex1d Bounds(Vector3 mainAxis, Vector3 crossAxis, Vector3 normalAxis, double length)
    {
        _isBoundsSet = true;
        //Line line = new(origin, mainAxis, length);
        //Bounds(line, crossAxis);
        Vector3 start = mainAxis.Unitized() * length * 0.5 * -1;
        Vector3 end = mainAxis.Unitized() * length * 0.5;
        _curve = new Line(start, end);
        Orientation(mainAxis, crossAxis, normalAxis);
        return this;
    }

    /// <summary>
    /// Set the container bounds.
    /// </summary>
    public Flex1d Bounds(Arc arc)
    {
        _isBoundsSet = true;
        _curve = arc;
        return this;
    }

    /// <inheritdoc cref="FlexBase._mainJustification"/>
    public Flex1d MainJustification(Justification justification)
    {
        _mainJustification = justification;
        return this;
    }

    /// <inheritdoc cref="FlexBase._crossAlignment"/>
    public Flex1d CrossAlignment(Alignment alignment)
    {
        _crossAlignment = alignment;
        return this;
    }

    /// <inheritdoc cref="FlexBase._normalAlignment"/>
    public Flex1d NormalAlignment(Alignment alignment)
    {
        _normalAlignment = alignment;
        return this;
    }

    /// <inheritdoc cref="FlexBase._crossSettingOut"/>
    public Flex1d CrossSettingOut(Alignment settingOut)
    {
        _crossSettingOut = settingOut;
        return this;
    }

    /// <inheritdoc cref="FlexBase._normalSettingOut"/>
    public Flex1d NormalSettingOut(Alignment settingOut)
    {
        _normalSettingOut = settingOut;
        return this;
    }

    /// <inheritdoc cref="_pattern"/>
    public Flex1d Pattern(SequenceBuilder sequence)
    {
        _pattern = sequence;
        return this;
    }

    #endregion

    #region Execution methods

    /// <summary>
    /// Execute the builder and assign <see cref="_results"/>.
    /// </summary>
    public Flex1d Build()
    {
        if (_pattern is null)
            throw new($"Failed to build {nameof(Flex1d)}. Pattern is not set.");
        if (!_isBoundsSet)
            throw new($"Failed to build {nameof(Flex1d)}. Bounds are not set.");
        Proxy[] components = _pattern.Build(this).ToArray();
        components = ApplyJustification(components, _mainJustification);
        components = ApplyAlignment(components, _crossAlignment, _crossAxis);
        components = ApplyAlignment(components, _normalAlignment, _normalAxis);
        components = ApplySettingOut(components, _crossSettingOut, _crossAxis);
        components = ApplySettingOut(components, _normalSettingOut, _normalAxis);
        _results = components;
        return this;
    }

    /// <summary>
    /// Execute the builder and create instances of the results placed along <see cref="_curve"/>.
    /// </summary>
    public IReadOnlyCollection<ElementInstance> ToComponents()
    {
        Build();

        return _results
            .Select((component, i) =>
            {
                Transform transform;
                switch (_curve)
                {
                    case Line line:
                        {
                            Vector3 origin = line.PointAt(0) + component.Translation;
                            transform = new(origin);
                            break;
                        }
                    case Arc:
                        {
                            double mainComponent = _mainAxis.Dimension(component.Translation);
                            double crossComponent = _crossAxis.Dimension(component.Translation);
                            double normalComponent = _normalAxis.Dimension(component.Translation);
                            Transform t = _curve.UnboundTransformAtLength(mainComponent);
                            Vector3 mainAxis = t.ZAxis.Negate();
                            Vector3 crossAxis = t.XAxis.Negate();
                            Vector3 normalAxis = t.YAxis;
                            Vector3 origin = t.Origin
                                             + crossAxis * crossComponent
                                             + normalAxis * normalComponent;

                            transform = new(origin, mainAxis, crossAxis, normalAxis);
                            break;
                        }
                    default:
                        throw new ArgumentException($"Failed to get components of {nameof(Flex1d)}. Only curve and line are accepted.");
                }

                return component
                    .BaseDefinition
                    .CreateInstance(transform, $"{component.BaseDefinition.Name}-{i}");
            })
            .ToArray();
    }

    #endregion

    #region Internal logic

    private Proxy[] ApplyJustification(Proxy[] components, Justification justification)
    {
        double remainder = MainRemainder(justification, components);
        double settingOut = JustificationSettingOut(justification, remainder, components.Length);
        double defaultSpacing = Spacing(justification, remainder, components.Length);
        double previousSpacing = 0;
        double coordination = 0;
        return components.Select((component, componentIndex) =>
            {
                double spacing = new[]
                {
                    defaultSpacing,
                    previousSpacing,
                    _mainAxis.Dimension(component.MinSpacing)
                }.Max();
                bool isFirst = componentIndex == 0;
                if (!isFirst)
                    component.Spacing = spacing * _mainAxis;

                component.SettingOut += settingOut * _mainAxis;

                coordination += _mainAxis.Dimension(component.Bounds) * 0.5;
                component.Coordination = coordination * _mainAxis;
                coordination += _mainAxis.Dimension(component.Bounds) * 0.5;

                if (!isFirst)
                    coordination += spacing;

                previousSpacing = spacing;
                return component;
            })
            .ToArray();
    }

    #region ignore

    private static Proxy[] ApplyAlignment(Proxy[] components, Alignment alignment, Vector3 axis)
    {
        double maxDimension = components.Max(component => CO.Minus(axis.Dimension(component.Bounds)));
        return components.Select(item =>
            {
                item.Alignment += Alignment(axis, alignment, item, maxDimension) * axis;
                return item;
            })
            .ToArray();
    }

    private static Proxy[] ApplySettingOut(Proxy[] components, Alignment settingOut, Vector3 axis)
    {
        double max = components.Max(component => CO.Minus(axis.Dimension(component.Bounds)));
        return components.Select(component =>
            {
                component.Alignment += SettingOut(settingOut, max) * axis;
                return component;
            })
            .ToArray();
    }

    internal double MainDimensionWithMinSpacing(IReadOnlyCollection<Proxy> components)
    {
        return components.Sum(component => CO.Nominal(_mainAxis.Dimension(component.Bounds), _mainAxis.Dimension(component.MinSpacing)))
               - _mainAxis.Dimension(components.Last().MinSpacing);
    }

    private double MainDimensionWithoutMinSpacing(IReadOnlyCollection<Proxy> components)
    {
        return components.Sum(component => CO.Minus(_mainAxis.Dimension(component.Bounds)));
    }
    #endregion

    private double MainRemainder(Justification justification, Proxy[] components)
    {
        double total = justification switch
        {
            Justification.Start
                or Justification.End
                or Justification.Center => MainDimensionWithMinSpacing(components),
            Justification.SpaceAround
                or Justification.SpaceBetween
                or Justification.SpaceEvenly => MainDimensionWithoutMinSpacing(components),
            _ => throw new EnumSwitchException<Justification>("Failed to get remainder.", justification),
        };
        return TargetLength - total;
    }

    private static double JustificationSettingOut(Justification justification, double remainder, int count)
    {
        return justification switch
        {
            Justification.Start => 0,
            Justification.End => remainder,
            Justification.Center => remainder * 0.5,
            Justification.SpaceAround => Spacing(justification, remainder, count) * 0.5,
            Justification.SpaceBetween => 0,
            Justification.SpaceEvenly => Spacing(justification, remainder, count),
            _ => throw new EnumSwitchException<Justification>("Failed to get justification setting out.", justification)
        };
    }

    private static double Spacing(Justification justification, double remainder, int count)
    {
        return justification switch
        {
            Justification.Start => 0,
            Justification.End => 0,
            Justification.Center => 0,
            Justification.SpaceAround => (remainder / (count * 2)) * 2,
            Justification.SpaceBetween => remainder / (count - 1),
            Justification.SpaceEvenly => remainder / (count + 1),
            _ => throw new EnumSwitchException<Justification>("Failed to get spacing.", justification)
        };
    }

    private static double Alignment(Vector3 axis, Alignment alignment, Proxy component, double max)
    {
        double remainder = max - CO.Minus(axis.Dimension(component.Bounds));
        return alignment switch
        {
            Flex.Alignment.Start => remainder * 0.5 * -1,
            Flex.Alignment.End => remainder * 0.5,
            Flex.Alignment.Center => 0,
            _ => throw new EnumSwitchException<Alignment>("Failed to get alignment.", alignment)
        };
    }

    private static double SettingOut(Alignment settingOut, double max)
    {
        return settingOut switch
        {
            Flex.Alignment.Start => max * 0.5,
            Flex.Alignment.End => max * 0.5 * -1,
            Flex.Alignment.Center => 0,
            _ => throw new EnumSwitchException<Alignment>("Failed to get setting out.", settingOut)
        };
    }

    #endregion
}
