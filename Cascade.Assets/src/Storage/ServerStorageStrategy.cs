using System.IO;
using Cascade.Assets.Visualization;
using Geometrician.Core.Storage;
using StudioLE.Core.Results;

namespace Cascade.Assets.Storage;

/// <summary>
/// A strategy to store files in the local file system and serve them from Cascade.Server
/// </summary>
public class ServerStorageStrategy : IStorageStrategy
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
            using FileStream fileStream = new(absolutePath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
            stream.Close();
            stream.Dispose();
            string uri = VisualizationConfiguration.BaseAddress + "/" + VisualizationConfiguration.StorageRoute + "/" + fileName;
            return new Success<Uri>(new(uri));
        }
        catch (Exception e)
        {
            return new Failure<Uri>("Failed to write to file storage.", e);
        }
    }
}
