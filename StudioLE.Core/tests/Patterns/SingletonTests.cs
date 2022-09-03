using NUnit.Framework;

namespace StudioLE.Core.Tests.Patterns;

internal sealed class SingletonTests
{
    [Test]
    public void Singleton_GetInstance()
    {
        // Arrange
        // const string message = "Hello, world.";
        SingletonA.CreateInstance = () => new();
        SingletonB.CreateInstance = () => new();
        // SingletonC.CreateInstance = () => message;

        // Act
        Example nonSingleton = new();
        Example a1 = SingletonA.GetInstance();
        Example a2 = SingletonA.GetInstance();
        SingletonA.CreateInstance = () => throw new("If this is thrown then the singleton is not working.");
        Example a3 = SingletonA.GetInstance();
        Example b1 = SingletonB.GetInstance();
        Example b2 = SingletonB.GetInstance();
        // string c1 = SingletonC.GetInstance();
        // string c2 = SingletonC.GetInstance();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(a1.Id, Is.Not.EqualTo(default(Guid)), "A1 and default");
            Assert.That(a1.Id, Is.Not.EqualTo(nonSingleton.Id), "A1 and non-singleton");
            Assert.That(a1.Id, Is.EqualTo(a2.Id), "A1 and A2");
            Assert.That(a1.Id, Is.EqualTo(a3.Id), "A1 and A3");
            Assert.That(b1.Id, Is.EqualTo(b2.Id), "B1 and B2");
            Assert.That(a1.Id, Is.Not.EqualTo(b1.Id), "A1 and B1");
            // Assert.That(c1, Is.EqualTo(message), "C1 and message constant");
            // Assert.That(c1, Is.EqualTo(c2), "C1 and C2");
            // Assert.That(a1, Is.Not.EqualTo(c2), "A1 and C1");
        });
    }

    private class Example
    {
        public Guid Id { get; }

        public Example()
        {
            Id = Guid.NewGuid();
        }
    }

    private static class SingletonA
    {
        private static readonly object _lock = new();
        private static Example? _instance;

        /// <summary>
        /// The method used to construct an instance if it does not exist.
        /// </summary>
        public static Func<Example> CreateInstance { get; set; } = () => new();

        /// <summary>
        /// Get a singleton instance.
        /// </summary>
        public static Example GetInstance()
        {
            if (_instance is not null)
                return _instance;

            if (CreateInstance is null)
                throw new("Failed to get singleton instance. CreateInstance is not set.");

            lock (_lock)
                _instance ??= CreateInstance.Invoke();

            return _instance;
        }
    }

    private static class SingletonB
    {
        private static readonly object _lock = new();
        private static Example? _instance;

        /// <summary>
        /// The method used to construct an instance if it does not exist.
        /// </summary>
        public static Func<Example> CreateInstance { get; set; } = () => new();

        /// <summary>
        /// Get a singleton instance.
        /// </summary>
        public static Example GetInstance()
        {
            if (_instance is not null)
                return _instance;

            if (CreateInstance is null)
                throw new("Failed to get singleton instance. CreateInstance is not set.");

            lock (_lock)
                _instance ??= CreateInstance.Invoke();

            return _instance;
        }
    }
}
