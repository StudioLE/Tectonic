using Lineweights.Diagnostics.NUnit.Visualization;

namespace Lineweights.Flex.Tests;

internal sealed class WallSamples
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public async Task Wall_StretcherBond()
    {
        // Arrange
        Samples.WallStretcherBond.Inputs inputs = new();

        // Act
        Samples.WallStretcherBond.Outputs outputs = Samples.WallStretcherBond.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
        IEnumerable<ElementInstance> components = outputs.Model.AllElementsOfType<ElementInstance>();
        await Verify.ElementsByBounds(components);
    }

    [Test]
    public async Task Wall_FlemishBond()
    {
        // Arrange
        Samples.WallFlemishBond.Inputs inputs = new();

        // Act
        Samples.WallFlemishBond.Outputs outputs = Samples.WallFlemishBond.Execute(inputs);

        // Preview
        _model.AddElements(outputs.Model.Elements.Values);

        // Assert
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
