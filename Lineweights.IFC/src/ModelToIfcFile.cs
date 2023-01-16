using System.IO;
using Elements.Serialization.IFC;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Results;

namespace Lineweights.IFC;

/// <summary>
/// Convert a <see cref="Model"/> to an IFC file
/// referenced as <see cref="IAsset"/>.
/// </summary>
public class ModelToIfcFile : IConverter<Model, Task<IResult<Uri>>>
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly string _fileName;


    /// <inheritdoc cref="ModelToIfcFile"/>
    public ModelToIfcFile(IStorageStrategy storageStrategy, string fileName)
    {
        _storageStrategy = storageStrategy;
        _fileName = fileName;
    }

    /// <inheritdoc/>
    public async Task<IResult<Uri>> Convert(Model model)
    {
        string tempPath = Path.GetTempFileName();

        // Temporarily capture the console output from ToIFC
        string console;
        using (StringWriter consoleWriter = new())
        {
            Console.SetOut(consoleWriter);
            model.ToIFC(tempPath);
            console = consoleWriter.ToString();
        }

        // Reset the console output
        StreamWriter standardOutputWriter = new(Console.OpenStandardOutput())
        {
            AutoFlush = true
        };
        Console.SetOut(standardOutputWriter);

        if (!File.Exists(tempPath))
            throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
        Stream stream = File.OpenRead(tempPath);
        IResult<Uri> result = await _storageStrategy.WriteAsync(_fileName, stream);
        if (!string.IsNullOrWhiteSpace(console))
            result.Warnings = new[] { console };
        return result;
    }
}
