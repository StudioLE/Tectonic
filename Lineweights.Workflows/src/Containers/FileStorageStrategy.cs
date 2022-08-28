using System.IO;
using Ardalis.Result;

namespace Lineweights.Workflows.Containers;

/// <summary>
/// A strategy to store files in the local file system.
/// </summary>
public class FileStorageStrategy : IStorageStrategy
{
    private readonly string _directory = Path.GetTempPath();

    /// <inheritdoc/>
    public async Task<Result<Uri>> WriteAsync(string fileName, Stream stream)
    {
        try
        {
            string absolutePath = Path.Combine(_directory, fileName);
            if (File.Exists(absolutePath))
                return Result<Uri>.Error("Failed to write to file storage. The file already exists.");
            using var fileStream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
            stream.Close();
            stream.Dispose();
            return new Uri(absolutePath);
        }
        catch (Exception e)
        {
            return Result<Uri>.Error("Failed to write to file storage.", e.Message);
        }
    }
}
