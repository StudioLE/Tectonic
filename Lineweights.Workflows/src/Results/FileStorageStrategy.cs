using System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A strategy to store files in the local file system.
/// </summary>
public class FileStorageStrategy : IStorageStrategy
{
    private readonly string _directory = Path.GetTempPath();

    /// <summary>
    /// Upload asynchronously to blob storage via a stream.
    /// </summary>
    public async Task<Result> WriteAsync(
        DocumentInformation doc,
        string fileExtension,
        string? mimeType,
        Func<Result, Stream> source)
    {
        Result result = new()
        {
            Metadata = doc
        };
        try
        {
            string fileName = doc.Id + fileExtension;

            FileInfo file = new(Path.Combine(_directory, fileName));
            while (file.Exists)
            {
                fileName = Guid.NewGuid() + fileExtension;
                file = new(Path.Combine(_directory, fileName));
            }

            doc.Location = new(file.FullName);

            Stream stream = source.Invoke(result);

            using var fileStream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            result.Errors = new[]
            {
                "Failed to write.",
                e.Message
            };
        }
        return result;
    }
}
