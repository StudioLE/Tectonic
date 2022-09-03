using System.IO;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Hosting;
using Lineweights.Workflows.NUnit.Visualization;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Lineweights.Workflows.Tests.Visualization;

internal sealed class VisualizationTests
{
    private readonly IServiceProvider _services;
    private readonly Model _model;

    public VisualizationTests()
    {
        _services = Services.GetInstance();
        _model = new();
        _model.AddElements(Scenes.Brickwork());
    }

    [Test]
    public async Task VisualizeAsFile_by_DI()
    {
        // Arrange
        VisualizeAsFile strategy = _services.GetRequiredService<VisualizeAsFile>();
        strategy.IsOpenEnabled = false;

        // Act
        Asset asset = await strategy.Execute(_model, new());

        // Assert
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        string? path = asset.Children.First().Info.Location?.LocalPath;
        Assert.That(File.Exists(path), "File exists");
    }

    [TestCase(typeof(VisualizeAsFile))]
    [TestCase(typeof(VisualizeWithGeometricianServer))]
    public void VisualizeAfterTestAttribute_by_Type(Type type)
    {
        // Arrange
        // Act
        VisualizeAfterTestAttribute attribute = new(type);

        // Assert
        Assert.That(attribute.Strategy, Is.TypeOf(type), "Strategy");
    }

    [Test]
    [Explicit("Requires SignalR")]
    [Category("Requires SignalR")]
    public async Task VisualizeWithGeometricianServer_by_DI()
    {
        // Arrange
        VisualizeWithGeometricianServer strategy = _services.GetRequiredService<VisualizeWithGeometricianServer>();

        // Act
        Asset asset = await strategy.Execute(_model, new());

        // Assert
        Assert.That(strategy.State, Is.EqualTo(HubConnectionState.Connected), "Connection state");
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
    }

    [Test]
    [Explicit("Requires SignalR")]
    [Category("Requires SignalR")]
    [VisualizeAfterTest(typeof(VisualizeWithGeometricianServer), IsEnabled = true)]
    public void VisualizeInServerAppAfterTest_UsingAttribute()
    {
    }
}
