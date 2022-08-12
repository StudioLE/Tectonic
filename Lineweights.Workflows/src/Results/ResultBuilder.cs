using System.IO;
using Elements.Serialization.glTF;
using Elements.Serialization.IFC;
using Lineweights.Drawings;
using Lineweights.PDF.From.Elements;
using Lineweights.SVG.From.Elements;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Results;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/builder">builder</see> to create a <see cref="Result"/>.
/// </summary>
public class ResultBuilder
{
    private readonly Result _result = new();

    /// <inheritdoc cref="Result"/>
    public ResultBuilder Metadata(DocumentInformation metadata)
    {
        _result.Metadata = metadata;

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddDocumentInformation(Model model)
    {
        foreach (DocumentInformation metadata in model.AllElementsOfType<DocumentInformation>())
        {
            Result result = new()
            {
                Metadata = metadata
            };
            _result.Children.Add(result);
        }

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToGlb(Model model, DocumentInformation? metadata = null)
    {
        metadata ??= new() { Name = "GlTF of Model" };
        FileInfo file = PathHelpers.GetTempFile(".glb");
        // TODO: Skip if no GeometricElement
        model.ToGlTF(file.FullName, out List<BaseError> errors);

        metadata.Location = new(file.FullName);
        Result result = new() { Metadata = metadata };
        if (errors.Any())
        {
            result.Errors = errors
                .Select(x => x.Message)
                .Prepend("Failed to convert Model to GLB.")
                .ToArray();
        }
        _result.Children.Add(result);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToIfc(Model model, DocumentInformation? metadata = null)
    {
        metadata ??= new() { Name = "IFC of Model" };
        FileInfo file = PathHelpers.GetTempFile(".ifc");
        using StringWriter sw = new();
        Console.SetOut(sw);
        model.ToIFC(file.FullName);
        string console = sw.ToString();
        metadata.Location = new(file.FullName);
        Result result = new() { Metadata = metadata };
        if (!string.IsNullOrWhiteSpace(console))
        {
            result.Errors = new[]
            {
                "Failed to convert Model to GLB.", console
            };
        }
        _result.Children.Add(result);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public ResultBuilder AddModelConvertedToJson(Model model, DocumentInformation? metadata = null)
    {
        metadata ??= new() { Name = "JSON of Model" };
        FileInfo file = PathHelpers.GetTempFile(".json");
        model.ToJson(file.FullName);
        metadata.Location = new(file.FullName);
        Result result = new() { Metadata = metadata };
        _result.Children.Add(result);
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
        FileInfo file = PathHelpers.GetTempFile(".svg");
        CanvasToSvgFile converter = new();
        converter.Convert(canvas, file);

        Result result = new();
        result.Metadata = new()
        {
            Id = canvas.Id,
            Name = canvas.Name,
            Location = new(file.FullName)
        };
        _result.Children.Add(result);

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
        FileInfo file = PathHelpers.GetTempFile(".pdf");
        CanvasToPdfFile converter = new();
        converter.Convert(canvas, file);

        Result result = new();
        result.Metadata = new()
        {
            Id = canvas.Id,
            Name = canvas.Name,
            Location = new(file.FullName)
        };
        _result.Children.Add(result);

        return this;
    }

    /// <inheritdoc cref="Result"/>
    public Result Build()
    {
        return _result;
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
}
