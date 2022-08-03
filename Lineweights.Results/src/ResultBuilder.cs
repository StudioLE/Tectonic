using System.IO;
using Elements.Serialization.glTF;
using Elements.Serialization.IFC;
using Lineweights.Drawings;
using Lineweights.SVG.From.Elements;

namespace Lineweights.Results;

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
    public ResultBuilder AddModelConvertedToGlb(Model model, string title)
    {
        FileInfo file = GetTempFile(".glb");
        // TODO: Skip if no GeometricElement
        model.ToGlTF(file.FullName, out List<BaseError> errors);

        Result result = new();
        if (!errors.Any())
        {
            result.Metadata.Name = title;
            result.Metadata.Location = new(file.FullName);
        }
        else
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
    public ResultBuilder AddModelConvertedToIfc(Model model, string title)
    {
        FileInfo file = GetTempFile(".ifc");
        using StringWriter sw = new();
        Console.SetOut(sw);
        model.ToIFC(file.FullName);
        string console = sw.ToString();

        Result result = new();
        if (string.IsNullOrWhiteSpace(console))
        {
            result.Metadata.Name = title;
            result.Metadata.Location = new(file.FullName);
        }
        else
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
    public ResultBuilder AddModelConvertedToJson(Model model, string title)
    {
        FileInfo file = GetTempFile(".json");
        model.ToJson(file.FullName);

        Result result = new()
        {
            Metadata =
            {
                Name = title,
                Location = new(file.FullName)
            }
        };
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
        FileInfo file = GetTempFile(".svg");
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
    public Result Build()
    {
        return _result;
    }

    private static FileInfo GetTempFile(string extension)
    {
        return new(Path.GetTempFileName() + extension);
    }

    /// <inheritdoc cref="Result"/>
    public static Result Default(Model model, DocumentInformation metadata)
    {
        return new ResultBuilder()
            .Metadata(metadata)
            .AddModelConvertedToGlb(model, metadata.Name)
            .AddCanvasesConvertedToSvg(model)
            .Build();
    }
}
