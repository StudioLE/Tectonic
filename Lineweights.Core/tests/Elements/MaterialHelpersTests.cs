namespace Lineweights.Core.Tests.Elements;

[SendToDashboardAfterTest]
internal sealed class MaterialHelpersTests : ResultModel
{
    [TestCase("blue", "bed58e79-27fb-5c9e-a631-6489bd1bf316")]
    [TestCase("green", "27b63f21-9576-5303-94c1-dbc3c704d273")]
    [TestCase("red", "d9bb1476-daf3-536a-a827-a6090b121ee7")]
    public void MaterialHelpers_MaterialByName(string hexOrName, string expectedId)
    {
        // Arrange
        var box1 = new Box(new(0, 0, 0), new Vector3(1, 1, 1));
        var box2 = new Box(new(0, 2, 0), new Vector3(1, 3, 1));

        // Act
        Material material1 = MaterialByName(hexOrName);
        Material material2 = MaterialByName(hexOrName);

        // Preview
        Model.AddElements(box1.ToModelCurves(material1));
        Model.AddElements(box2.ToModelCurves(material2));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(material1.Name, Is.EqualTo(hexOrName));
            Assert.That(material1.Id, Is.EqualTo(new Guid(expectedId)));
            Assert.That(material1, Is.EqualTo(material2));
            Assert.That(material1.Name, Is.EqualTo(material2.Name));
            Assert.That(material1.Id, Is.EqualTo(material2.Id));
        });
    }
}
