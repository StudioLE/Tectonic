using System.IO;
using System.Reflection;
using Ardalis.Result;
using Microsoft.Extensions.FileProviders;

namespace Lineweights.Workflows.Samples;

/// <summary>
/// Sample scenes.
/// </summary>
public static class Scenes
{
    /// <summary>
    /// Scene names.
    /// </summary>
    public enum Name
    {
        /// <summary>
        /// A sample scene of brickwork.
        /// </summary>
        Brickwork,

        /// <summary>
        /// A sample scene of geometric elements.
        /// </summary>
        GeometricElements,

        /// <summary>
        /// A sample scene of geometric elements with views and a sheet.
        /// </summary>
        GeometricElementsOnSheet
    }

    /// <inheritdoc cref="Name.Brickwork"/>
    internal static ElementInstance[] Brickwork()
    {
        return FromJson(Name.Brickwork)
            .AllElementsOfType<ElementInstance>()
            .ToArray();
    }

    /// <inheritdoc cref="Name.GeometricElements"/>
    internal static GeometricElement[] GeometricElements()
    {
        return FromJson(Name.GeometricElements)
            .AllElementsOfType<GeometricElement>()
            .ToArray();
    }

    /// <summary>
    /// Load a sample model from JSON.
    /// </summary>
    internal static Model FromJson(Name name)
    {
        Assembly assembly = typeof(Scenes).Assembly;
        EmbeddedFileProvider provider = new(assembly);
        IFileInfo file = provider.GetFileInfo("Resources/" + name + ".json");
        if (!file.Exists)
            throw new("The file does not exist.");
        using Stream stream = file.CreateReadStream();
        using StreamReader reader = new(stream);
        string json = reader.ReadToEnd();
        Result<Model> result = ModelHelpers.TryGetFromJson(json);
        return Validate.OrThrow(result, $"Failed to load scene {name} from JSON");
    }

    /// <summary>
    /// Load a sample model from JSON.
    /// </summary>
    internal static FileInfo ToJson(Name name, IEnumerable<Element> elements)
    {
        throw new NotImplementedException();
        FileInfo file = new(name + ".json");
        Model model = new();
        model.AddElements(elements);
        string json = model.ToJson(true);
        System.IO.File.WriteAllText(file.FullName, json);
        return file;
    }
}
