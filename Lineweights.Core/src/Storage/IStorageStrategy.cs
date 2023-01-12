using System.IO;
using StudioLE.Core.Results;

namespace Lineweights.Core.Storage;

/// <summary>
/// A strategy to store files.
/// </summary>
public interface IStorageStrategy
{
    /// <summary>
    /// Write a file asynchronously via a stream.
    /// </summary>
    Task<IResult<Uri>> WriteAsync(string fileName, Stream stream);
}
