using System.IO;
using System.Xml.Linq;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    // TODO: BlobStorage should be injected
    // TODO: Add a local file storage alternative strategy for OpenAsFile. Must override AsyncUpload
    private const string BlobConnectionString = "UseDevelopmentStorage=true";
    private const string BlobContainer = "dashboard";
    private static readonly BlobClientOptions _blobOptions = new()
    {
        Retry =
        {
            MaxRetries = 0,
            NetworkTimeout = TimeSpan.FromMilliseconds(300),
            Delay = TimeSpan.Zero,
            MaxDelay = TimeSpan.Zero,
            Mode = RetryMode.Fixed
        },
    };
    private readonly BlobContainerClient _container = new(BlobConnectionString, BlobContainer, _blobOptions);
    private readonly List<Task<Result>> _results = new();
    private DocumentInformation? _metadata;

    /// <inheritdoc cref="Result"/>
    public ResultBuilder Metadata(DocumentInformation metadata)
    {
        _metadata = metadata;

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddDocumentInformation(Model model)
    {
        foreach (DocumentInformation metadata in model.AllElementsOfType<DocumentInformation>())
            AddDocumentInformation(metadata);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddDocumentInformation(DocumentInformation metadata)
    {
        if (metadata.Location is null)
            throw new("Failed to upload DocumentInformation. The document location is null.");
        if (!metadata.Location.IsFile)
            throw new("Failed to upload DocumentInformation. The document is not a file.");
        string filePath = metadata.Location.AbsolutePath;
        string fileExtension = GetFileExtension(metadata.Location);
        Task<Result> task = UploadAsync(metadata, fileExtension, null, result =>
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Failed to upload DocumentInformation. The file does not exist.");
            return File.OpenRead(filePath);
        });

        _results.Add(task);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToGlb(Model model, DocumentInformation? metadata = null)
    {
        metadata ??= new() { Name = "GlTF of Model" };

        if (!model.AllElementsOfType<GeometricElement>().Any())
            return this;

        Task<Result> task = UploadAsync(metadata, ".glb", "model/gltf-binary", result =>
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
    public ResultBuilder AddModelConvertedToIfc(Model model, DocumentInformation? metadata = null)
    {
        metadata ??= new() { Name = "IFC of Model" };

        Task<Result> task = UploadAsync(metadata, ".ifc", "application/x-step", result =>
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
    public ResultBuilder AddModelConvertedToJson(Model model, DocumentInformation? metadata = null)
    {
        metadata ??= new() { Name = "JSON of Model" };

        Task<Result> task = UploadAsync(metadata, ".json", "application/json", result =>
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
        DocumentInformation metadata = new()
        {
            Id = canvas.Id,
            Name = canvas.Name
        };

        Task<Result> task = UploadAsync(metadata, ".svg", "image/svg+xml", result =>
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
        DocumentInformation metadata = new()
        {
            Id = canvas.Id,
            Name = canvas.Name
        };

        Task<Result> task = UploadAsync(metadata, ".pdf", "application/pdf", result =>
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
            Metadata = _metadata ?? new(),
            Children = result
        };
    }

    /// <inheritdoc cref="Result"/>
    public static Result Default(Model model, DocumentInformation metadata)
    {
        return new ResultBuilder()
            .Metadata(metadata)
            .AddModelConvertedToGlb(model)
            .AddModelConvertedToIfc(model)
            .AddCanvasesConvertedToPdf(model)
            .AddCanvasesConvertedToSvg(model)
            .AddDocumentInformation(model)
            .AddModelConvertedToJson(model)
            .Build();
    }

    /// <summary>
    /// Upload asynchronously to blob storage via a stream.
    /// </summary>
    private async Task<Result> UploadAsync(
        DocumentInformation metadata,
        string fileExtension,
        string? mimeType,
        Func<Result, Stream> source)
    {
        Result result = new()
        {
            Metadata = metadata
        };
        try
        {
            string fileName = metadata.Id + fileExtension;
            BlobClient blob = _container.GetBlobClient(fileName);
            while (blob.Exists())
            {
                fileName = Guid.NewGuid() + fileExtension;
                blob = _container.GetBlobClient(fileName);
            }

            metadata.Location = blob.Uri;

            Stream stream = source.Invoke(result);
            BlobHttpHeaders headers = mimeType is not null
                ? new()
                {
                    ContentType = mimeType
                }
                : new();
            await blob.UploadAsync(stream, headers).ConfigureAwait(false);
            stream.Close();
            stream.Dispose();
        }
        catch (Exception e)
        {
            result.Errors = new[]
            {
                "Failed to upload.",
                e.Message
            };
        }
        return result;
    }

    private static string GetFileExtension(Uri uri)
    {
        string fileName = uri.Segments.Last();
        string fileExtension = fileName.Split('.').Last();
        return "." + fileExtension;
    }
}
