using System.IO;
using Elements.Serialization.glTF;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Lineweights.Core.Converters;

/// <summary>
/// Convert a <see cref="Model"/> to a GLB file
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class ModelToGlbFile : IConverter<Model, Task<IResult<Uri>>>
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly string _fileName;

    /// <inheritdoc cref="ModelToGlbFile"/>
    public ModelToGlbFile(IStorageStrategy storageStrategy, string fileName)
    {
        _storageStrategy = storageStrategy;
        _fileName = fileName;
    }

    /// <inheritdoc/>
    public async Task<IResult<Uri>> Convert(Model model)
    {
        string tempPath = Path.GetTempFileName();
        model.ToGlTF(tempPath, out List<BaseError> conversionErrors);
        if (!File.Exists(tempPath))
            throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
        Stream stream = File.OpenRead(tempPath);
        IResult<Uri> result = await _storageStrategy.WriteAsync(_fileName, stream);
        if (conversionErrors.Any())
            result.Warnings = conversionErrors.Select(x => x.Message).ToArray();
        return result;
    }
}
