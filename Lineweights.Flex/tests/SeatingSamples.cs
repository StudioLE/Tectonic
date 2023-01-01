using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

internal sealed class SeatingSamples
{
    private readonly Model _model = new();

    [Test]
    public async Task Seating_1d_Linear()
    {
        // Arrange
        Samples.Seating1dLinear.Inputs inputs = new();

        // Act
        Samples.Seating1dLinear.Outputs outputs = Samples.Seating1dLinear.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);
    }

    [Test]
    public async Task Seating_1d_Radial()
    {
        // Arrange
        Samples.Seating1dRadial.Inputs inputs = new();

        // Act
        Samples.Seating1dRadial.Outputs outputs = Samples.Seating1dRadial.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);
    }

    [Test]
    public async Task Seating_2d_Alternating()
    {
        // Arrange
        Samples.Seating2dAlternating.Inputs inputs = new();

        // Act
        Samples.Seating2dAlternating.Outputs outputs = Samples.Seating2dAlternating.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);
    }

    [TearDown]
    public async Task TearDown()
    {
        await new Visualize().Execute(_model);
    }
}
