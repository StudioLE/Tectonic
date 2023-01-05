using Lineweights.Workflows.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

internal sealed class FlexSamples
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public async Task Flex2d_Sample()
    {
        // Arrange
        Samples.Flex2dSample.ContainerInputs containerInputs = new();
        Samples.Flex2dSample.SequenceInputs firstSequenceInputs = new();
        Samples.Flex2dSample.SequenceInputs secondSequenceInputs = new();
        Samples.Flex2dSample.FlexInputs flexInputs = new();
        Samples.Flex2dSample.DisplayInputs displayInputs = new();

        // Act
        Samples.Flex2dSample.Outputs outputs = Samples.Flex2dSample.Execute(
            containerInputs,
            firstSequenceInputs,
            secondSequenceInputs,
            flexInputs,
            displayInputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        Assert.That(outputs, Is.Not.Null, "Outputs");
        Assert.That(outputs.Model.Elements.Count, Is.EqualTo(526), "Outputs model count");
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);
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
