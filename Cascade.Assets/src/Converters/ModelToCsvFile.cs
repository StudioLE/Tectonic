using System.Globalization;
using System.IO;
using CsvHelper;
using Elements;
using StudioLE.Patterns;
using StudioLE.Results;
using StudioLE.Storage;

namespace Cascade.Assets.Converters;

/// <summary>
/// Convert a <see cref="Model"/> to a CSV file.
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
