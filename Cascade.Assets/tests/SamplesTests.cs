using Geometrician.Core.Assets;
using Geometrician.Diagnostics.NUnit.Visualization;
using Geometrician.Diagnostics.Samples;
using Geometrician.Drawings;
using Cascade.Assets.Samples;
using Elements;
using NUnit.Framework;

namespace Cascade.Assets.Tests;

internal sealed class SamplesTests
{
    private readonly Visualize _visualize = new();
    private Model _model = new();

    [Test]
    public async Task AssetTypes([Values] Scenes.Name name, [Values] bool includeAssetsInModel)
    {
        // Arrange
        AssetTypes.Inputs inputs = new()
        {
            Scene = name,
            IncludeAssetsInModel = includeAssetsInModel
        };

        // Act
        AssetTypes.Outputs outputs = await new AssetTypes().Execute(inputs);

        // Preview
        _model = outputs.Model;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(outputs.Model.AllElementsOfType<View>().Count(), Is.EqualTo(3), "View count");
            Assert.That(outputs.Model.AllElementsOfType<Sheet>().Count(), Is.EqualTo(1), "Sheet count");
            if (inputs.IncludeAssetsInModel)
            {
                Assert.That(outputs.Model.AllElementsOfType<ExternalAsset>().Count(), Is.EqualTo(1), "ExternalAsset count");
                Assert.That(outputs.Model.AllElementsOfType<InternalAsset>().Count(), Is.EqualTo(1), "InternalAsset count");
                Assert.That(outputs.ExternalAsset, Is.Null, "ExternalAsset");
                Assert.That(outputs.InternalAsset, Is.Null, "InternalAsset");
            }
            else
            {
                Assert.That(outputs.Model.AllElementsOfType<ExternalAsset>().Count(), Is.EqualTo(0), "ExternalAsset count");
                Assert.That(outputs.Model.AllElementsOfType<InternalAsset>().Count(), Is.EqualTo(0), "InternalAsset count");
                Assert.That(outputs.ExternalAsset, Is.Not.Null, "ExternalAsset");
                Assert.That(outputs.InternalAsset, Is.Not.Null, "InternalAsset");
            }
        });
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
