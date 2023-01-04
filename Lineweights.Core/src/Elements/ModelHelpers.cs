using System.Reflection;
using Lineweights.Core.Elements.Comparers;

namespace Lineweights.Core.Elements;

/// <summary>
/// Methods to extend <see cref="Model"/>.
/// </summary>
public static class ModelHelpers
{
    internal static void AddSubElements(Model @this)
    {
        ElementIdComparer comparer = new();
        IReadOnlyCollection<Element> subElements = @this
            .Elements
            .Values
            .SelectMany(x => x.GetSubElements())
            .Distinct(comparer)
            .Where(x => !@this.Elements.ContainsKey(x.Id))
            .ToArray();
        @this.AddElements(subElements, false);
    }

    public static void AddSubElements(this Model @this, Element element)
    {
        ElementIdComparer comparer = new();
        IReadOnlyCollection<Element> subElements = element
            .GetSubElements()
            .Distinct(comparer)
            .Where(x => !@this.Elements.ContainsKey(x.Id))
            .ToArray();
        @this.AddElements(subElements, false);
    }

    private static IEnumerable<Element> GetSubElements(this Element element)
    {
        PropertyInfo[] properties = element.GetType().GetProperties();
        return properties
            .SelectMany(property => Array.Empty<Element>()
                .Concat(element.GetSubElementsIfElement(property))
                .Concat(element.GetSubElementsIfEnumerable(property))
                .Concat(element.GetSubElementsIfRepresentation(property)));
    }

    private static IEnumerable<Element> GetSubElementsIfElement(this Element element, PropertyInfo property)
    {
        bool isElement = typeof(Element).IsAssignableFrom(property.PropertyType);
        if (!isElement)
            return Enumerable.Empty<Element>();
        Element subElement = (Element)property.GetValue(element);
        return subElement
            .GetSubElements()
            .Append(subElement);
    }

    private static IEnumerable<Element> GetSubElementsIfEnumerable(this Element element, PropertyInfo property)
    {
        bool isElementEnumerable = typeof(IEnumerable<Element>).IsAssignableFrom(property.PropertyType);
        if (!isElementEnumerable)
            return Enumerable.Empty<Element>();
        IReadOnlyCollection<Element> subElements = ((IEnumerable<Element>)property.GetValue(element)).ToArray();
        return subElements
            .SelectMany(x => x.GetSubElements())
            .Concat(subElements);

    }

    private static IEnumerable<Element> GetSubElementsIfRepresentation(this Element element, PropertyInfo property)
    {
        bool isRepresentation = typeof(Representation).IsAssignableFrom(property.PropertyType);
        if (!isRepresentation)
            return Enumerable.Empty<Element>();
        Representation? representation = (Representation?)property.GetValue(element);
        if (representation is null)
            return Enumerable.Empty<Element>();
        return representation
            .SolidOperations
            .SelectMany(solidOperation => solidOperation switch
            {
                Extrude extrude => new[] { extrude.Profile },
                Sweep sweep => new[] { sweep.Profile },
                _ => Array.Empty<Element>()
            });
    }
}
