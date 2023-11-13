using System.IO;
using Cascade.Assets.Storage;
using NUnit.Framework;
using StudioLE.Results;
using StudioLE.Extensions.System;
using StudioLE.Storage;

namespace Cascade.Assets.Tests.Documents;

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
        IResult<Uri> result = await storageStrategy.WriteAsync(fileName, stream);
        if (result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success<Uri>, "Result is success.");
        if (result is Success<Uri> success)
        {
            Assert.That(success.Value.IsFile, "Uri is file");
            Assert.That(File.Exists(success.Value.AbsolutePath), "File exists");
        }
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
        IResult<Uri> result = await storageStrategy.WriteAsync(fileName, stream);
        if (result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success<Uri>, "Result is success.");
        if (result is Success<Uri> success)
            Assert.That(success.Value.IsFile, Is.False, "Uri is file");
        if (result.Errors.Any())
            foreach (string error in result.Errors)
                Console.WriteLine(error);
    }
}
