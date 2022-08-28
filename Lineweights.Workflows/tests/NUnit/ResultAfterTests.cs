using System.IO;
using Lineweights.Workflows.Containers;
using Lineweights.Workflows.Results;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lineweights.Workflows.Tests.NUnit;

internal sealed class ResultAfterTests : ResultModel
{
    [SetUp]
    public void Setup()
    {
        Model.AddElements(
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
    public async Task ResultAfterTest_OpenAsFile(string fileExtension)
    {
        // Arrange
        OpenAsFileAfterTest attribute = new(fileExtension)
        {
            IsEnabled = true
        };

        if (attribute.Strategy is not OpenAsFile strategy)
        {
            Assert.Fail("Strategy type");
            return;
        }

        strategy.IsOpenEnabled = false;

        // Act
        Container container = await strategy.Execute(Model, new());

        // Assert
        Assert.That(container.Children.Count, Is.EqualTo(1), "Children count");
        string? path = container.Children.First().Info.Location?.LocalPath;
        Assert.That(path, Does.EndWith($"{fileExtension}"), "File extension");
        Assert.That(File.Exists(path), "File exists");
    }

    [Test]
    [Explicit("Requires Dashboard")]
    [Category("Dashboard")]
    public async Task ResultAfterTest_SendToDashboard()
    {
        // Arrange
        SendToDashboardAfterTest attribute = new()
        {
            IsEnabled = true
        };
        if (attribute.Strategy is not SendToDashboard strategy)
        {
            Assert.Fail("Strategy type");
            return;
        }

        // Act
        Container container = await strategy.Execute(Model, new());

        // Assert
        Assert.That(strategy.State, Is.EqualTo(HubConnectionState.Connected), "Connection state");
        Assert.That(container.Children.Count, Is.EqualTo(1), "Children count");
    }

    [Test]
    [Explicit("Requires Dashboard")]
    [Category("Dashboard")]
    [SendToDashboardAfterTest(IsEnabled = true)]
    public void ResultAfterTest_SendToDashboard_Attribute()
    {
    }
}
