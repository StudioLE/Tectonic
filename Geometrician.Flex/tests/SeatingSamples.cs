using Geometrician.Diagnostics;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Flex.Samples;
using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Geometrician.Flex.Tests;

internal sealed class SeatingSamples
{
    private readonly Verify _verify = new(new NUnitVerifyContext());
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public async Task Seating_1d_Linear()
    {
        // Arrange
        Seating1dLinear.Inputs inputs = new();

        // Act
        Seating1dLinear.Outputs outputs = await new Seating1dLinear().Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await _verify.ElementsByBounds(components);
    }

    [Test]
    public async Task Seating_1d_Radial()
    {
        // Arrange
        Seating1dRadial.Inputs inputs = new();

        // Act
        Seating1dRadial.Outputs outputs = await new Seating1dRadial().Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await _verify.ElementsByBounds(components);
    }

    [Test]
    public async Task Seating_2d_Alternating()
    {
        // Arrange
        Seating2dAlternating.Inputs inputs = new();

        // Act
        Seating2dAlternating.Outputs outputs = await new Seating2dAlternating().Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await _verify.ElementsByBounds(components);
    }

    [TearDown]
    public void TearDown()
    {
        _visualize.Queue(_model);
        _model = new();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _visualize.Execute();
    }
}
