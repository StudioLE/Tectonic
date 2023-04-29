using System.IO;
using Geometrician.Core.Storage;
using StudioLE.Core.Results;
using StudioLE.Core.System.IO;

namespace Cascade.Components.Scripts;

/// <summary>
/// A strategy to store files as an
/// <see href="https://developer.mozilla.org/en-US/docs/Web/API/URL/createObjectURL">ObjectUrl</see>.
/// </summary>
public class ObjectUrlStorageStrategy : IStorageStrategy
{
    private readonly ObjectUrlStorage _facade;

    /// <inheritdoc cref="ObjectUrlStorage"/>
    public ObjectUrlStorageStrategy(ObjectUrlStorage facade)
    {
        _facade = facade;
    }

    /// <inheritdoc/>
    public async Task<IResult<Uri>> WriteAsync(string fileName, Stream stream)
    {
        try
        {
            string contentType = fileName.GetContentTypeByExtension() ?? "application/octet-stream";
            using MemoryStream ms = new();
            stream.CopyTo(ms);
            byte[] byteArray = ms.ToArray();
            Uri uri = await _facade.Create(fileName, contentType, byteArray);
            stream.Close();
            stream.Dispose();
            return new Success<Uri>(uri);
        }
        catch (Exception e)
        {
            return new Failure<Uri>(e.Message);
        }
    }
}
