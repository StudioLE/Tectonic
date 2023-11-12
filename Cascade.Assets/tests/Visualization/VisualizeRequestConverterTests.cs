using Cascade.Assets.Visualization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Cascade.Assets.Tests.Visualization;

internal sealed class VisualizeRequestConverterTests
{
    [Test]
    public void VisualizeRequestConverter_FromJson()
    {
        // Arrange
        string json = """
                      {
                      "Model": {
                        "Transform": {
                          "Matrix": {
                            "Components": [
                              1.0,
                              0.0,
                              0.0,
                              0.0,
                              0.0,
                              1.0,
                              0.0,
                              0.0,
                              0.0,
                              0.0,
                              1.0,
                              0.0
                            ]
                          }
                        },
                        "Elements": {}
                      },
                      "Assemblies": [
                          "/e/Repos/Elements/Cascade/Cascade.Server/tests/bin/Debug/net7.0/Geometrician.Drawings.dll"
                        ]
                      }
                      """;
        VisualizeRequest? deserialized = null;

        // Act
        Assert.DoesNotThrow(() => deserialized = JsonConvert.DeserializeObject<VisualizeRequest>(json));

        // Assert
        Assert.That(deserialized, Is.Not.Null, "Not null");
    }
}
