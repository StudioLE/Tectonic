using Lineweights.Core.Distribution;

namespace Lineweights.Flex;

/// <summary>
/// Methods to create <see cref="ElementInstance"/> from <see cref="Proxy"/>.
/// </summary>
public static class CreateInstance
{
    /// <summary>
    /// Create an <see cref="ElementInstance"/> of a <see cref="Proxy"/> on a line.
    /// </summary>
    internal static ElementInstance OnLine(Proxy component, Line line, string name)
    {
        Vector3 origin = line.PointAt(0) + component.Translation;
        Transform transform = new(origin);
        return component
            .BaseDefinition
            .CreateInstance(transform, name);
    }

    /// <summary>
    /// Create an <see cref="ElementInstance"/> of a <see cref="Proxy"/> on an arc.
    /// </summary>
    internal static ElementInstance OnArc(
        Proxy component,
        Arc arc,
        string name,
        // ReSharper disable InconsistentNaming
        Vector3 _mainAxis,
        Vector3 _crossAxis,
        Vector3 _normalAxis)
        // ReSharper restore InconsistentNaming
    {
        double mainComponent = _mainAxis.Dimension(component.Translation);
        double crossComponent = _crossAxis.Dimension(component.Translation);
        double normalComponent = _normalAxis.Dimension(component.Translation);
        Transform t = arc.UnboundTransformAtLength(mainComponent);
        Vector3 mainAxis = t.ZAxis.Negate();
        Vector3 crossAxis = t.XAxis.Negate();
        Vector3 normalAxis = t.YAxis;
        Vector3 origin = t.Origin
                         + crossAxis * crossComponent
                         + normalAxis * normalComponent;

        Transform transform = new(origin, mainAxis, crossAxis, normalAxis);
        return component
            .BaseDefinition
            .CreateInstance(transform, name);
    }
}
