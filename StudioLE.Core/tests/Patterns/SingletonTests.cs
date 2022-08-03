using NUnit.Framework;
using StudioLE.Core.Patterns;

namespace StudioLE.Core.Tests.Patterns;

internal sealed class SingletonTests
{
    [Test]
    public void Singleton_Construct()
    {
        // Arrange
        Func<Guid> constructor = Guid.NewGuid;

        // Act
        Guid instance1 = Singleton<Guid>.GetInstance(constructor);
        Guid instance2 = Singleton<Guid>.GetInstance(constructor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(instance1, Is.EqualTo(instance2), "Singleton instances are equal.");
            Assert.That(instance1, Is.Not.EqualTo(constructor()), "Non-singleton instances are different.");
        });
    }
}
