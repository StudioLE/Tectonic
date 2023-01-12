using System.IO;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Core.Converters;

/// <summary>
/// Convert a <see cref="Model"/> to a JSON file
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class ModelToJsonFile : IConverter<Model, Task<IResult<Uri>>>
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly string _fileName;

    public ModelToJsonFile(IStorageStrategy storageStrategy, string fileName)
    {
        _storageStrategy = storageStrategy;
        _fileName = fileName;
    }

    /// <inheritdoc />
    public Task<IResult<Uri>> Convert(Model model)
    {
        string json = model.ToJson();
        using MemoryStream stream = new();
        using StreamWriter writer = new(stream);
        writer.WriteAsync(json);
        return _storageStrategy.WriteAsync(_fileName, stream);
    }
}
