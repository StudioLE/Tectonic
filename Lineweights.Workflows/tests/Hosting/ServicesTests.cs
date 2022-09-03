using Lineweights.Core.Documents;
using Lineweights.Workflows.Execution;
using Lineweights.Workflows.Hosting;
using Lineweights.Workflows.Visualization;

namespace Lineweights.Workflows.Tests.Hosting;

internal sealed class ServicesTests
{
    [TestCase(typeof(GeometricianService))]
    [TestCase(typeof(IVisualizationStrategy))]
    [TestCase(typeof(IActivityFactory))]
    [TestCase(typeof(IStorageStrategy))]
    [TestCase(typeof(IAssetBuilder))]
    public void Services_GetService(Type type)
    {
        // Arrange
        IServiceProvider services = Services.GetInstance();

        // Act
        object? service =  services.GetService(type);

        // Assert
        Assert.That(service, Is.Not.Null, "Service");
    }
}
