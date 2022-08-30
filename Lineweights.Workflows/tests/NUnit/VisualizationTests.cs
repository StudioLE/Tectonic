using System.IO;
using Lineweights.Workflows.Assets;
using Lineweights.Workflows.NUnit.Visualization;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lineweights.Workflows.Tests.NUnit;

internal sealed class VisualizationTests
{
    private readonly Model _model = new();

    #region Implement ModelTest

    /// <summary>
    /// A test <see cref="Line"/>.
    /// </summary>
    public static Line TestLine = new(Vector3.Origin, new(5, 5, 5));

    /// <summary>
    /// A test <see cref="Arc"/>.
    /// </summary>
    public static Arc TestArc = new(Vector3.Origin, 2.0, 0.0, 90.0);

    /// <summary>
    /// A test <see cref="Polyline"/>..
    /// </summary>
    public static Polyline TestPolyline = new(new Vector3(0, 0), new Vector3(0, 2), new Vector3(0, 3, 1));

    /// <summary>
    /// A test <see cref="Polygon"/>.
    /// </summary>
    public static Polygon TestPolygon = Polygon.Ngon(5, 2);

    /// <summary>
    /// A test <see cref="Circle"/>.
    /// </summary>
    public static Circle TestCircle = new(Vector3.Origin, 5);

    #endregion

    [SetUp]
    public void Setup()
    {
        _model.AddElements(
            //CreateModelArrows.ByTransform(new()),
            CreateBBox3.ByLengths(3, 2, 1).ToMass(new(3, 2, 1), BuiltInMaterials.Default),
            TestArc,
            TestCircle,
            TestLine,
            TestPolygon,
            TestPolyline
        );
    }

    [TestCase(".glb")]
    [TestCase(".ifc")]
    [TestCase(".json")]
    public async Task VisualizeAsFileAfterTest(string fileExtension)
    {
        // Arrange
        VisualizeAsFileAfterTestAttribute attribute = new(fileExtension)
        {
            IsEnabled = true
        };

        if (attribute.Strategy is not VisualizeAsFile strategy)
        {
            Assert.Fail("Strategy type");
            return;
        }

        strategy.IsOpenEnabled = false;

        // Act
        Asset asset = await strategy.Execute(_model, new());

        // Assert
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
        string? path = asset.Children.First().Info.Location?.LocalPath;
        Assert.That(path, Does.EndWith($"{fileExtension}"), "File extension");
        Assert.That(File.Exists(path), "File exists");
    }

    [Test]
    [Explicit("Requires SignalR")]
    [Category("Requires SignalR")]
    public async Task VisualizeInServerAppAfterTest()
    {
        // Arrange
        VisualizeInServerAppAfterTestAttribute attribute = new()
        {
            IsEnabled = true
        };
        if (attribute.Strategy is not VisualizeInServerApp strategy)
        {
            Assert.Fail("Strategy type");
            return;
        }

        // Act
        Asset asset = await strategy.Execute(_model, new());

        // Assert
        Assert.That(strategy.State, Is.EqualTo(HubConnectionState.Connected), "Connection state");
        Assert.That(asset.Children.Count, Is.EqualTo(1), "Children count");
    }

    [Test]
    [Explicit("Requires SignalR")]
    [Category("Requires SignalR")]
    [VisualizeInServerAppAfterTest(IsEnabled = true)]
    public void VisualizeInServerAppAfterTest_UsingAttribute()
    {
    }
}
