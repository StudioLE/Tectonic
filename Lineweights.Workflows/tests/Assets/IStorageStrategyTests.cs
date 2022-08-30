using System.IO;
using Ardalis.Result;
using Lineweights.Workflows.Assets;

namespace Lineweights.Workflows.Tests.Assets;

internal sealed class StorageStrategyTests
{
    [Test]
    public async Task FileStorageStrategy_WriteAsync()
    {
        // Arrange
        IStorageStrategy storageStrategy = new FileStorageStrategy();
        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write("Hello, world.");
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        string fileName = Guid.NewGuid() + ".txt";

        // Act
        Result<Uri> result = await storageStrategy.WriteAsync(fileName, stream);
        Uri uri = result.Value;

        // Assert
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(uri.IsFile, "Uri is file");
        Assert.That(File.Exists(uri.AbsolutePath), "File exists");
        if(result.Errors.Any())
            foreach (string error in result.Errors)
                Console.WriteLine(error);
    }

    [Test]
    [Explicit("Requires Azurite")]
    [Category("Requires Azurite")]
    public async Task BlobStorageStrategy_WriteAsync()
    {
        // Arrange
        IStorageStrategy storageStrategy = new BlobStorageStrategy();
        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        writer.Write("Hello, world.");
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        string fileName = Guid.NewGuid() + ".txt";

        // Act
        Result<Uri> result = await storageStrategy.WriteAsync(fileName, stream);
        Uri uri = result.Value;

        // Assert
        Assert.That(uri, Is.Not.Null, "Uri is not null");
        Assert.That(!uri.IsFile, "Uri is not file");
        if(result.Errors.Any())
            foreach (string error in result.Errors)
                Console.WriteLine(error);
    }
}
