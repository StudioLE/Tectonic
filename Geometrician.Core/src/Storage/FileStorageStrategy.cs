using System.IO;
using Lineweights.Core.Storage;
using StudioLE.Core.Results;

namespace Geometrician.Core.Storage;

/// <summary>
/// A strategy to store files in the local file system.
/// </summary>
public class FileStorageStrategy : IStorageStrategy
{
    private readonly string _directory = Path.GetTempPath();

    /// <inheritdoc/>
    public async Task<IResult<Uri>> WriteAsync(string fileName, Stream stream)
    {
        try
        {
            string absolutePath = Path.Combine(_directory, fileName);
            if (File.Exists(absolutePath))
                return new Failure<Uri>("Failed to write to file storage. The file already exists.");
            using var fileStream = new FileStream(absolutePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
            stream.Close();
            stream.Dispose();
            return new Success<Uri>(new(absolutePath));
        }
        catch (Exception e)
        {
            return new Failure<Uri>("Failed to write to file storage.", e);
        }
    }
}
