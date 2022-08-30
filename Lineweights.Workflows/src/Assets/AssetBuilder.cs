using System.IO;
using System.Xml.Linq;
using Elements.Serialization.glTF;
using Elements.Serialization.IFC;
using Lineweights.Drawings;
using Lineweights.PDF;
using Lineweights.PDF.From.Elements;
using Lineweights.SVG;
using Lineweights.SVG.From.Elements;
using QuestPDF.Fluent;
using StudioLE.Core.Exceptions;

namespace Lineweights.Workflows.Assets;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create a <see cref="Asset"/>.
/// </summary>
public class AssetBuilder
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly List<Task<Asset>> _tasks = new();
    private readonly DocumentInformation _doc;

    public AssetBuilder(IStorageStrategy storageStrategy, DocumentInformation? doc = null)
    {
        _storageStrategy = storageStrategy;
        _doc = doc ?? new();
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ConvertModelToGlb(Model model, DocumentInformation? doc = null)
    {
        doc ??= new() { Name = "GlTF of Model" };

        if (!model.AllElementsOfType<GeometricElement>().Any())
            return this;

        string fileName = doc.Id + ".glb";
        string tempPath = Path.GetTempFileName();
        model.ToGlTF(tempPath, out List<BaseError> errors);

        Asset asset = new()
        {
            Info = doc,
            ContentType =  "model/gltf-binary"
        };
        if (errors.Any())
            asset.Errors = errors
                .Select(x => x.Message)
                .Prepend("Failed to convert Model to GLB.")
                .ToArray();

        if (!File.Exists(tempPath))
            throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
        Stream stream =  File.OpenRead(tempPath);

        _tasks.Add(_storageStrategy.WriteAsync(asset, fileName, stream));
        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ConvertModelToIfc(Model model, DocumentInformation? doc = null)
    {
        doc ??= new() { Name = "IFC of Model" };

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
        StreamWriter standardOutputWriter = new (Console.OpenStandardOutput());
        standardOutputWriter.AutoFlush = true;
        Console.SetOut(standardOutputWriter);

        Asset asset = new()
        {
            Info = doc,
            ContentType =  "application/x-step"
        };
        if (!string.IsNullOrWhiteSpace(console))
            asset.Errors = new[]
            {
                "Failed to convert Model to IFC.",
                console
            };

        if (!File.Exists(tempPath))
            throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
        Stream stream = File.OpenRead(tempPath);

        string fileName = doc.Id + ".ifc";
        _tasks.Add(_storageStrategy.WriteAsync(asset, fileName, stream));
        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ConvertModelToJson(Model model, DocumentInformation? doc = null)
    {
        doc ??= new() { Name = "JSON of Model" };

        string json = model.ToJson();

        Asset asset = new()
        {
            Info = doc,
            ContentType =  "application/json",
            Content = json
        };
        _tasks.Add(Task.FromResult(asset));
        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ExtractSheetsAndConvertToSvg(Model model)
    {
        foreach (Canvas canvas in model.AllElementsOfType<Sheet>())
            ConvertCanvasToSvg(canvas);

        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ExtractViewsAndConvertToSvg(Model model)
    {
        foreach (Canvas canvas in model.AllElementsOfType<View>())
            ConvertCanvasToSvg(canvas);

        return this;
    }

    /// <inheritdoc cref="Asset"/>
    private AssetBuilder ConvertCanvasToSvg(Canvas canvas)
    {

        SvgDocument svgDocument = canvas switch
        {
            // TODO: If new() works for View then we should amend SheetToSvg to also return the SVG element.
            Sheet sheet => new SheetToSvg().Convert(sheet),
            View view => new(new ViewToSvg().Convert(view)),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to svg.", canvas)
        };
        MemoryStream stream = new();
        svgDocument.Save(stream, SaveOptions.None);
        stream.Seek(0, SeekOrigin.Begin);

        Asset asset = new()
        {
            Info = new()
            {
                Id = canvas.Id,
                Name = canvas.Name
            },
            ContentType =  "image/svg+xml"
        };
        string fileName = asset.Info.Id + ".svg";
        _tasks.Add(_storageStrategy.WriteAsync(asset, fileName, stream));
        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ExtractSheetsAndConvertToPdf(Model model)
    {
        foreach (Canvas canvas in model.AllElementsOfType<Sheet>())
            ConvertCanvasToPdf(canvas);

        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public AssetBuilder ExtractViewsAndConvertToPdf(Model model)
    {
        foreach (Canvas canvas in model.AllElementsOfType<View>())
            ConvertCanvasToPdf(canvas);

        return this;
    }

    /// <inheritdoc cref="Asset"/>
    private AssetBuilder ConvertCanvasToPdf(Canvas canvas)
    {
        PdfDocument pdfDocument = canvas switch
        {
            Sheet sheet => new SheetToPdf().Convert(sheet),
            View view => new ViewToPdf().Convert(view),
            _ => throw new TypeSwitchException<Canvas>("Failed to convert canvas to pdf.", canvas)
        };
        MemoryStream stream = new();
        pdfDocument.GeneratePdf(stream);
        stream.Seek(0, SeekOrigin.Begin);

        Asset asset = new()
        {
            Info = new()
            {
                Id = canvas.Id,
                Name = canvas.Name
            },
            ContentType =  "application/pdf"
        };
        string fileName = asset.Info.Id + ".pdf";
        _tasks.Add(_storageStrategy.WriteAsync(asset, fileName, stream));
        return this;
    }

    /// <inheritdoc cref="Asset"/>
    public async Task<Asset> Build()
    {
        Asset[] assets = await Task.WhenAll(_tasks);
        return new()
        {
            Info = _doc,
            Children = assets
        };
    }

    /// <inheritdoc cref="Asset"/>
    public static AssetBuilder Default(IStorageStrategy storageStrategy, Model model, DocumentInformation? doc = null)
    {
        return new AssetBuilder(storageStrategy, doc)
            .ConvertModelToGlb(model)
            .ExtractViewsAndConvertToSvg(model)
            .ExtractSheetsAndConvertToPdf(model);
    }
}
