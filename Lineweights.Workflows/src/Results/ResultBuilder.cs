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

namespace Lineweights.Workflows.Results;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create a <see cref="Result"/>.
/// </summary>
public class ResultBuilder
{
    private readonly IStorageStrategy _storageStrategy;
    private readonly List<Task<Result>> _results = new();
    private readonly DocumentInformation _doc;

    public ResultBuilder(IStorageStrategy storageStrategy, DocumentInformation? doc = null)
    {
        _storageStrategy = storageStrategy;
        _doc = doc ?? new();
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddDocumentInformation(Model model)
    {
        foreach (DocumentInformation doc in model.AllElementsOfType<DocumentInformation>())
            AddDocumentInformation(doc);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddDocumentInformation(DocumentInformation doc)
    {
        if (doc.Location is null)
            throw new("Failed to upload DocumentInformation. The document location is null.");
        if (!doc.Location.IsFile)
            throw new("Failed to upload DocumentInformation. The document is not a file.");
        string filePath = doc.Location.AbsolutePath;
        string fileExtension = GetFileExtension(doc.Location);
        Task<Result> task = _storageStrategy.WriteAsync(doc, fileExtension, result =>
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Failed to upload DocumentInformation. The file does not exist.");
            return File.OpenRead(filePath);
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToGlb(Model model, DocumentInformation? doc = null)
    {
        doc ??= new() { Name = "GlTF of Model" };

        if (!model.AllElementsOfType<GeometricElement>().Any())
            return this;

        Task<Result> task = _storageStrategy.WriteAsync(doc, ".glb", result =>
        {
            string tempPath = Path.GetTempFileName();

            model.ToGlTF(tempPath, out List<BaseError> errors);
            if (errors.Any())
            {
                result.Errors = errors
                    .Select(x => x.Message)
                    .Prepend("Failed to convert Model to GLB.")
                    .ToArray();
            }

            if (!File.Exists(tempPath))
                throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
            return File.OpenRead(tempPath);
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToIfc(Model model, DocumentInformation? doc = null)
    {
        doc ??= new() { Name = "IFC of Model" };

        Task<Result> task = _storageStrategy.WriteAsync(doc, ".ifc", result =>
        {
            string tempPath = Path.GetTempFileName();
            string console;
            using (StringWriter consoleWriter = new())
            {
                Console.SetOut(consoleWriter);
                model.ToIFC(tempPath);
                console = consoleWriter.ToString();
            }
            if (!string.IsNullOrWhiteSpace(console))
            {
                result.Errors = new[]
                {
                    "Failed to convert Model to IFC.",
                    console
                };
            }

            if (!File.Exists(tempPath))
                throw new FileNotFoundException("Failed to write GLB. Temp file does not exist.");
            return File.OpenRead(tempPath);
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToJson(Model model, DocumentInformation? doc = null)
    {
        doc ??= new() { Name = "JSON of Model" };

        Task<Result> task = _storageStrategy.WriteAsync(doc, ".json", result =>
        {
            string json = model.ToJson();
            MemoryStream stream = new();
            StreamWriter writer = new(stream);
            writer.Write(json);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddCanvasesConvertedToSvg(Model model)
    {
        foreach (Canvas canvas in model.AllElementsOfType<Canvas>())
            AddCanvasConvertedToSvg(canvas);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddCanvasConvertedToSvg(Canvas canvas)
    {
        DocumentInformation doc = new()
        {
            Id = canvas.Id,
            Name = canvas.Name
        };

        Task<Result> task = _storageStrategy.WriteAsync(doc, ".svg", result =>
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
            return stream;
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddCanvasesConvertedToPdf(Model model)
    {
        foreach (Canvas canvas in model.AllElementsOfType<Canvas>())
            AddCanvasConvertedToPdf(canvas);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddCanvasConvertedToPdf(Canvas canvas)
    {
        DocumentInformation doc = new()
        {
            Id = canvas.Id,
            Name = canvas.Name
        };

        Task<Result> task = _storageStrategy.WriteAsync(doc, ".pdf", result =>
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
            return stream;
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public Result Build()
    {
        Task<Result[]> task = Task.WhenAll(_results);
        Result[] result = task.GetAwaiter().GetResult();
        return new()
        {
            Info = _doc,
            Children = result
        };
    }

    /// <inheritdoc cref="Result"/>
    public static Result Default(IStorageStrategy storageStrategy, Model model, DocumentInformation? doc = null)
    {
        return new ResultBuilder(storageStrategy, doc)
            .AddModelConvertedToGlb(model)
            .AddDocumentInformation(model)
            .Build();
    }

    private static string GetFileExtension(Uri uri)
    {
        string fileName = uri.Segments.Last();
        string fileExtension = fileName.Split('.').Last();
        return "." + fileExtension;
    }
}
