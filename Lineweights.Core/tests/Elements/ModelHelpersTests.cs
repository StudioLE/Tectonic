using System.Diagnostics.CodeAnalysis;

namespace Lineweights.Core.Tests.Elements;

internal sealed class ModelHelpersTests
{

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class TestElement : Element
    {
        public IReadOnlyCollection<Material> Materials { get; set; } = Array.Empty<Material>();
    }

    [Test]
    public void ModelHelpers_AddSubElements_OfElement()
    {
        // Arrange
        Material[] materials =
        {
            MaterialByName("Red"),
            MaterialByName("Green"),
            MaterialByName("Blue")
        };
        TestElement testElement = new()
        {
            Materials = materials
        };
        Model model = new();
        model.AddElement(testElement);

        int countBefore = model.Elements.Count;
        Console.WriteLine($"Before: {countBefore} elements");
        foreach(Element element in model.Elements.Values)
            Console.WriteLine($"{element.Id} {element.GetType()}");

        // Act
        model.AddSubElements(testElement);

        int countAfter = model.Elements.Count;
        Console.WriteLine($"After: {countAfter} elements");
        foreach(Element element in model.Elements.Values)
            Console.WriteLine($"{element.Id} {element.GetType()}");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(countBefore, Is.EqualTo(1), "Count before.");
            Assert.That(countAfter, Is.EqualTo(4), "Count after.");
        });
    }

    [Test]
    public void ModelHelpers_AddSubElements_All()
    {
        // Arrange
        Material[] materials =
        {
            MaterialByName("Red"),
            MaterialByName("Green"),
            MaterialByName("Blue")
        };
        TestElement testElement = new()
        {
            Materials = materials
        };
        Model model = new();
        model.AddElement(testElement);

        int countBefore = model.Elements.Count;
        Console.WriteLine($"Before: {countBefore} elements");
        foreach(Element element in model.Elements.Values)
            Console.WriteLine($"{element.Id} {element.GetType()}");

        // Act
        ModelHelpers.AddSubElements(model);

        int countAfter = model.Elements.Count;
        Console.WriteLine($"After: {countAfter} elements");
        foreach(Element element in model.Elements.Values)
            Console.WriteLine($"{element.Id} {element.GetType()}");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(countBefore, Is.EqualTo(1), "Count before.");
            Assert.That(countAfter, Is.EqualTo(4), "Count after.");
        });
    }
}
