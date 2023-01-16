using System.Globalization;
using System.IO;
using CsvHelper;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;

namespace Geometrician.Core.Converters;

/// <summary>
/// Convert a <see cref="Model"/> to a CSV file
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class ModelToCsvFile<TRow> : IConverter<Model, Task<IResult<Uri>>>
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly string _fileName;
    private readonly Func<Model, IReadOnlyCollection<TRow>> _method;

    /// <inheritdoc cref="ModelToCsvFile{T}"/>
    public ModelToCsvFile(IStorageStrategy storageStrategy, string fileName, Func<Model, IReadOnlyCollection<TRow>> method)
    {
        _storageStrategy = storageStrategy;
        _fileName = fileName;
        _method = method;
    }

    /// <inheritdoc/>
    public async Task<IResult<Uri>> Convert(Model model)
    {
        IReadOnlyCollection<TRow> table = _method.Invoke(model);
        MemoryStream stream = new();
        using StreamWriter writer = new(stream);
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(table);
        await writer.FlushAsync();
        stream.Seek(0, SeekOrigin.Begin);
        IResult<Uri> result = await _storageStrategy.WriteAsync(_fileName, stream);
        return result;
    }
}
