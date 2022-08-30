using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

[VisualizeInServerAppAfterTest]
internal sealed class SeatingSamples
{
    public Model Model { get; } = new();

    [Test]
    public void Seating_1d_Linear()
    {
        // Arrange
        Samples.Seating1dLinear.Inputs inputs = new();

        // Act
        Samples.Seating1dLinear.Outputs outputs = Samples.Seating1dLinear.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }

    [Test]
    public void Seating_1d_Radial()
    {
        // Arrange
        Samples.Seating1dRadial.Inputs inputs = new();

        // Act
        Samples.Seating1dRadial.Outputs outputs = Samples.Seating1dRadial.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }

    [Test]
    public void Seating_2d_Alternating()
    {
        // Arrange
        Samples.Seating2dAlternating.Inputs inputs = new();

        // Act
        Samples.Seating2dAlternating.Outputs outputs = Samples.Seating2dAlternating.Execute(inputs);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        Verify.ElementsByBounds(components);

        // Preview
        Model.AddElements(outputs.Model.Elements.Values);
    }
}
