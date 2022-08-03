using Newtonsoft.Json;

namespace Lineweights.Results.Tests;

internal sealed class SerialisationTests : ResultModel
{
    [Test]
    public void Serialisation_Result()
    {
        // Arrange
        Model model = new();

        Result result = ResultBuilder.Default(model, new());

        // Act
        string json = JsonConvert.SerializeObject(result);
        Result? deserialised = JsonConvert.DeserializeObject<Result>(json);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(deserialised, Is.Not.Null, "Not null");
            Assert.That(deserialised?.Metadata.Id, Is.EqualTo(result.Metadata.Id), "Parent Metadata Id");
            Result? glb = result.Children.FirstOrDefault();
            Result? deserialisedGlb = deserialised?.Children.FirstOrDefault();
            Assert.That(deserialisedGlb, Is.Not.Null, "Child not null");
            Assert.That(deserialisedGlb?.Metadata.Id, Is.EqualTo(glb?.Metadata.Id), "Child Metadata Id");
        });
    }
}
